using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PricingCalculator
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private PropertyDescriptor _sortProperty;
        private ListSortDirection _sortDirection;

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => _isSorted;

        protected override PropertyDescriptor SortPropertyCore => _sortProperty;

        protected override ListSortDirection SortDirectionCore => _sortDirection;

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            List<T> itemsList = (List<T>)Items;
            itemsList.Sort(Compare);

            _isSorted = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private int Compare(T x, T y)
        {
            var result = CompareValues(
                _sortProperty.GetValue(x),
                _sortProperty.GetValue(y));

            return _sortDirection == ListSortDirection.Ascending ? result : -result;
        }

        private int CompareValues(object x, object y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;

            if (x is IComparable comparableX && y is IComparable comparableY)
                return comparableX.CompareTo(y);

            return x.Equals(y) ? 0 : -1;
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _sortProperty = null;
        }
    }

    public partial class MainForm : Form
    {
        private SortableBindingList<PricingItem> items = new SortableBindingList<PricingItem>();
        private bool dollarStore = false;
        private EnumPricingStrategy globalStrategy = EnumPricingStrategy.None;
        private const string STORAGE_FILE = "pricing-data.json";
        private DataGridView dataGridView;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private EnumSeason currentSeason = EnumSeason.Spring;
        private ComboBox seasonCombo;
        private Label seasonLabel;
        private PricingCalculator calculator;

        private PricingGraph pricingGraph;

        private void InitializeReasonableConfigPanel()
        {
            // Create a container panel for the config sliders
            var configPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 500, // Increased height to give more space for the graph
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false, // Initially hidden until user wants to configure
                Padding = new Padding(5)
            };

            // Add a button to show/hide the config panel
            var configButton = new Button
            {
                Text = "Reasonable Increase Settings",
                Dock = DockStyle.Top,
                Height = 30,
            };

            configButton.Click += (s, e) =>
            {
                configPanel.Visible = !configPanel.Visible;
                configButton.Text = configPanel.Visible ?
                    "Hide Reasonable Increase Settings" :
                    "Reasonable Increase Settings";
            };

            // Create layout for sliders
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top, // Changed from Fill to Top
                Height = 250, // Fixed height for sliders
                ColumnCount = 3,
                RowCount = 9,
                Padding = new Padding(5),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Set column widths
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Label
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F)); // Slider
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F)); // Value

            // Add row styles to make everything more compact
            for (int i = 0; i < 9; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F)); // More compact rows
            }


            // Add sliders for each parameter
            AddCompactSlider(layout, 0, "Base Increase (%)", 1, 10,
                (int)(calculator.ReasonableConfig.BaseIncrease * 100),
                value =>
                {
                    calculator.ReasonableConfig.BaseIncrease = value / 100m;
                    UpdateGraph();
                    UpdateGrid();
                });

            AddCompactSlider(layout, 1, "Max Increase (%)", 5, 20,
                (int)(calculator.ReasonableConfig.MaxIncrease * 100),
                value =>
                {
                    calculator.ReasonableConfig.MaxIncrease = value / 100m;
                    UpdateGraph();
                    UpdateGrid();
                });

            AddCompactSlider(layout, 2, "Low Price Threshold ($)", 1, 30,
                (int)calculator.ReasonableConfig.LowPriceThreshold,
                value =>
                {
                    calculator.ReasonableConfig.LowPriceThreshold = value;
                    UpdateGraph();
                    UpdateGrid();
                });

            AddCompactSlider(layout, 3, "Mid Price Threshold ($)", 20, 150,
                (int)calculator.ReasonableConfig.MidPriceThreshold,
                value =>
                {
                    calculator.ReasonableConfig.MidPriceThreshold = value;
                    UpdateGraph();
                    UpdateGrid();
                });

            AddCompactSlider(layout, 4, "High Price Threshold ($)", 50, 500,
                (int)calculator.ReasonableConfig.HighPriceThreshold,
                value =>
                {
                    calculator.ReasonableConfig.HighPriceThreshold = value;
                    UpdateGraph();
                    UpdateGrid();
                });

            AddCompactSlider(layout, 5, "Low Price Add (%)", 1, 15,
                (int)(calculator.ReasonableConfig.LowPriceAdditional * 100),
                value =>
                {
                    calculator.ReasonableConfig.LowPriceAdditional = value / 100m;
                    UpdateGraph();
                    UpdateGrid();
                });

            AddCompactSlider(layout, 6, "Mid Price Add (%)", 1, 10,
                (int)(calculator.ReasonableConfig.MidPriceAdditional * 100),
                value =>
                {
                    calculator.ReasonableConfig.MidPriceAdditional = value / 100m;
                    UpdateGraph();
                    UpdateGrid();
                });

            AddCompactSlider(layout, 7, "High Price Add (%)", 0, 5,
                (int)(calculator.ReasonableConfig.HighPriceAdditional * 100),
                value =>
                {
                    calculator.ReasonableConfig.HighPriceAdditional = value / 100m;
                    UpdateGraph();
                    UpdateGrid();
                });

            // Add a checkbox for interpolation - span across all columns
            var interpolationCheckBox = new CheckBox
            {
                Text = "Use smooth interpolation between price tiers",
                Checked = calculator.ReasonableConfig.UseInterpolation,
                AutoSize = true,
                Margin = new Padding(3, 3, 0, 0)
            };

            interpolationCheckBox.CheckedChanged += (s, e) =>
            {
                calculator.ReasonableConfig.UseInterpolation = interpolationCheckBox.Checked;
                UpdateGraph();
                UpdateGrid();
            };

            // Add checkbox to last row
            layout.Controls.Add(interpolationCheckBox, 0, 8);
            layout.SetColumnSpan(interpolationCheckBox, 3);

            // Create graph panel
            pricingGraph = new PricingGraph
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                MinimumSize = new Size(300, 200)
            };

            // Set the configuration
            pricingGraph.SetConfig(calculator.ReasonableConfig);

            // Add controls to the config panel in the correct order
            configPanel.Controls.Add(pricingGraph); // Graph at the bottom (added first for Dock.Fill)
            configPanel.Controls.Add(layout); // Sliders at the top

            // Add to the form in the appropriate position
            var insertIndex = Controls.IndexOf(dataGridView);
            if (insertIndex >= 0)
            {
                Controls.Add(configPanel);
                Controls.Add(configButton);

                Controls.SetChildIndex(configPanel, insertIndex);
                Controls.SetChildIndex(configButton, insertIndex);
            }
            else
            {
                // Fallback - add at the top
                Controls.Add(configPanel);
                Controls.Add(configButton);
                configPanel.BringToFront();
                configButton.BringToFront();
            }
        }

        // Update the graph method
        private void UpdateGraph()
        {
            if (pricingGraph != null)
            {
                pricingGraph.SetConfig(calculator.ReasonableConfig);
                pricingGraph.Invalidate();

                // Debug information
                System.Diagnostics.Debug.WriteLine($"UpdateGraph called. Graph size: {pricingGraph.Width}x{pricingGraph.Height}");
            }
        }
        public MainForm()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeSeasonSettings();
            InitializeReasonableConfigPanel();
            LoadData();
        }

        private void UpdateCalculator()
        {
            if (calculator != null)
            {
                calculator.CurrentSeason = currentSeason;
                calculator.DollarStore = dollarStore;
            }
            else calculator = new PricingCalculator(currentSeason, dollarStore);
        }


        private void AddCompactSlider(TableLayoutPanel layout, int row, string labelText,
                             int min, int max, int initialValue, Action<int> valueChanged)
        {
            // Create label with more compact format
            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            // Create slider control with reduced height
            var slider = new TrackBar
            {
                Minimum = min,
                Maximum = max,
                Value = initialValue,
                Height = 25,
                TickFrequency = (max - min) / 5,
                TickStyle = TickStyle.TopLeft,
                AutoSize = false
            };

            // Create value display label
            var valueLabel = new Label
            {
                Text = initialValue.ToString(),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 30,
                Anchor = AnchorStyles.None
            };

            // Add event handler
            slider.ValueChanged += (s, e) =>
            {
                valueLabel.Text = slider.Value.ToString();
                valueChanged(slider.Value);
            };

            // Add controls to layout directly
            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(slider, 1, row);
            layout.Controls.Add(valueLabel, 2, row);
        }

        private void InitializeComponent()
        {
            this.Text = "Pricing Calculator";
            this.Size = new System.Drawing.Size(1200, 800);
            this.MinimumSize = new System.Drawing.Size(800, 600);

            // Create status strip
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusLabel.Text = "Ready";
            statusStrip.Items.Add(statusLabel);

            // Create main layout panel
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(10),
                AutoSize = true
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // For status bar

            // Header
            var headerPanel = new Panel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 0, 0, 10)
            };

            // Button Panel
            var buttonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 0, 0, 10),
                WrapContents = false
            };

            var exportButton = new Button
            {
                Text = "Export",
                Width = 100,
                Height = 30,
                Margin = new Padding(0, 0, 10, 0)
            };
            exportButton.Click += ExportData;

            var importButton = new Button
            {
                Text = "Import",
                Width = 100,
                Height = 30,
                Margin = new Padding(0, 0, 10, 0)
            };
            importButton.Click += ImportData;

            var clearButton = new Button
            {
                Text = "Clear All",
                Width = 100,
                Height = 30
            };
            clearButton.Click += ClearAllData;

            buttonPanel.Controls.AddRange(new Control[] { exportButton, importButton, clearButton });

            // Global Settings Panel
            var settingsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 0, 0, 10),
                WrapContents = false
            };

            var strategyLabel = new Label
            {
                Text = "Global Strategy:",
                AutoSize = true,
                Margin = new Padding(0, 5, 10, 0)
            };

            var strategyCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200,
                Height = 30
            };

            strategyCombo.Items.AddRange(Enum.GetNames(typeof(EnumPricingStrategy)));

            strategyCombo.SelectedIndex = 0;
            strategyCombo.SelectedIndexChanged += (s, e) =>
            {
                Enum.TryParse(strategyCombo.SelectedItem.ToString(), out globalStrategy);
                UpdateGrid();
            };

            var dollarStoreCheck = new CheckBox
            {
                Text = "Round all prices to whole dollars",
                AutoSize = true,
                Margin = new Padding(20, 5, 0, 0)
            };
            dollarStoreCheck.CheckedChanged += (s, e) =>
            {
                dollarStore = dollarStoreCheck.Checked;
                UpdateGrid();
            };

            seasonLabel = new Label
            {
                Text = "Current Season:",
                AutoSize = true,
                Margin = new Padding(20, 5, 10, 0)
            };

            seasonCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 150,
                Height = 30
            };

            seasonCombo.Items.AddRange(Enum.GetNames(typeof(EnumSeason)));
            seasonCombo.SelectedIndex = (int)currentSeason;
            seasonCombo.SelectedIndexChanged += (s, e) =>
            {
                Enum.TryParse(seasonCombo.SelectedItem.ToString(), out currentSeason);
                UpdateGrid();
            };

            settingsPanel.Controls.AddRange(new Control[] { strategyLabel, strategyCombo, dollarStoreCheck, seasonLabel, seasonCombo });

            // Grid
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = SystemColors.Window
            };

            // Add Button
            var addButton = new Button
            {
                Text = "Add Item",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            addButton.Click += (s, e) => AddRow();

            // Add controls to main panel
            mainPanel.Controls.Add(headerPanel, 0, 0);
            mainPanel.Controls.Add(buttonPanel, 0, 1);
            mainPanel.Controls.Add(settingsPanel, 0, 2);
            mainPanel.Controls.Add(dataGridView, 0, 3);
            mainPanel.Controls.Add(addButton, 0, 4);
            mainPanel.Controls.Add(statusStrip, 0, 5);

            this.Controls.Add(mainPanel);
        }

        private void InitializeGrid()
        {
            dataGridView.DataSource = items;

            // Enable sorting
            dataGridView.AllowUserToOrderColumns = true;
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;

            // Name Column
            var nameColumn = new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "Item Name",
                MinimumWidth = 150,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 25,
                SortMode = DataGridViewColumnSortMode.Automatic // Enable sorting
            };

            // Unit Cost Column
            var unitCostColumn = new DataGridViewTextBoxColumn
            {
                Name = "UnitCost",
                DataPropertyName = "UnitCost",
                HeaderText = "Unit Cost ($)",
                MinimumWidth = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight },
                SortMode = DataGridViewColumnSortMode.Automatic // Enable sorting
            };

            // Retail Price Column
            var retailPriceColumn = new DataGridViewTextBoxColumn
            {
                Name = "RetailPrice",
                DataPropertyName = "RetailPrice",
                HeaderText = "Market Price ($)",
                MinimumWidth = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight },
                SortMode = DataGridViewColumnSortMode.Automatic // Enable sorting
            };

            // Recommended Price Column
            var recommendedColumn = new DataGridViewTextBoxColumn
            {
                Name = "RecommendedPrice",
                DataPropertyName = "RecommendedPrice", // Bind to the property
                HeaderText = "Recommended",
                MinimumWidth = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 15,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight },
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            // Margin Column
            var marginColumn = new DataGridViewTextBoxColumn
            {
                Name = "MarginPercent",
                DataPropertyName = "MarginPercent",
                HeaderText = "Margin",
                MinimumWidth = 80,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 10,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "P2", // This formats as percentage with 2 decimal places
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            // vs Market Column
            var vsMarketColumn = new DataGridViewTextBoxColumn
            {
                Name = "PriceRelativeToMarket",
                DataPropertyName = "PriceRelativeToMarket",
                HeaderText = "vs Market",
                MinimumWidth = 80,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 10,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "P2", // This formats as percentage with 2 decimal places
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            // Add columns to the grid
            dataGridView.Columns.AddRange(new DataGridViewColumn[] {
                nameColumn,
                unitCostColumn,
                retailPriceColumn,
                recommendedColumn,
                marginColumn,
                vsMarketColumn
            });

            dataGridView.CellValueChanged += (s, e) => UpdateGridAtIndex(e.RowIndex);
            dataGridView.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dataGridView.IsCurrentCellDirty)
                {
                    dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };

            dataGridView.DataError += (s, e) =>
            {
                if (e.ColumnIndex == dataGridView.Columns["Strategy"].Index)
                {
                    var item = items[e.RowIndex];
                    e.ThrowException = false;
                }
            };

            dataGridView.CellValidating += (s, e) =>
            {
                // Skip validation for non-numeric columns
                if (e.ColumnIndex != dataGridView.Columns["UnitCost"].Index &&
                    e.ColumnIndex != dataGridView.Columns["RetailPrice"].Index)
                    return;

                if (dataGridView.Rows[e.RowIndex].IsNewRow) return;

                var value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                // Check if empty or not a valid number
                if (string.IsNullOrWhiteSpace(value) || !double.TryParse(value, out double numericValue) || numericValue < 0)
                {
                    e.Cancel = true;
                    dataGridView.Rows[e.RowIndex].ErrorText = "Please enter a valid positive number.";

                    // Highlight the cell with error
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightPink;
                }
            };

            dataGridView.CellEndEdit += (s, e) =>
            {
                // Clear error when editing is done
                dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = SystemColors.Window;
            };


            // Style the grid
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView.Font, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.RowTemplate.Height = 30;
            dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            for (int i = 0; i < items.Count; i++) UpdateGridAtIndex(i);
        }

        private void AddRow()
        {
            items.Add(new PricingItem());
        }

        private void UpdateGrid()
        {
            UpdateCalculator();

            for (int i = 0; i < items.Count; i++) UpdateGridAtIndex(i);
            SaveData();
        }

        private void UpdateGridAtIndexAndSave(int index)
        {
            UpdateGridAtIndex(index);
            SaveData();
        }
        private void UpdateGridAtIndex(int index)
        {
            var item = items[index];
            if (item == null) return;

            var result = CalculatePrice(item);
            if (result != null)
            {
                // Update the properties on the item
                item.RecommendedPrice = result.RecommendedPrice;

                // Store as actual decimal values (not multiplied by 100)
                item.MarginPercent = result.MarginPercent / 100m; // Convert to decimal percentage
                item.PriceRelativeToMarket = result.PriceRelativeToMarket / 100m; // Convert to decimal percentage
            }
            else
            {
                // Set default values
                item.RecommendedPrice = 0;
                item.MarginPercent = 0;
                item.PriceRelativeToMarket = 0;
            }
        }

        private const decimal minMarginPercent = 20;

        private EnumPricingFlag GetFlagsFromStrategy(EnumPricingStrategy strategy)
        {
            return (EnumPricingFlag)strategy;
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            // This lets the user place the cursor wherever they click
            // without interfering with normal click behavior
        }

        // Replace the old CalcByFlags method with this one to maintain backward compatibility during transition
        private decimal CalcByFlags(PricingItem item, EnumPricingFlag flags)
        {
            return calculator.CalculatePrice(item, flags);
        }

        private PriceCalculation CalculatePrice(PricingItem item)
        {
            // First validate inputs
            if (item == null || item.UnitCost <= 0 || item.RetailPrice <= 0)
            {
                return null;
            }

            try
            {
                decimal recommendedPrice = calculator.CalculatePrice(item, GetFlagsFromStrategy(globalStrategy));

                decimal marginPercent = ((recommendedPrice - item.UnitCost) / item.UnitCost) * 100;
                decimal priceRelativeToMarket = ((recommendedPrice / item.RetailPrice) - 1) * 100;
                return new PriceCalculation
                {
                    RecommendedPrice = recommendedPrice,
                    MarginPercent = Math.Round(marginPercent * 100) / 100,
                    PriceRelativeToMarket = Math.Round(priceRelativeToMarket * 100) / 100,
                    StrategyUsed = globalStrategy
                };
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error calculating price: {ex.Message}");
                return null;
            }
        }


        private void SaveDataToFile(string filePath)
        {
            try
            {
                var data = new SaveData
                {
                    Items = items.ToList(),
                    DollarStore = dollarStore,
                    GlobalStrategy = globalStrategy,
                    CurrentSeason = currentSeason,
                    ReasonableConfig = calculator.ReasonableConfig
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving data: {ex.Message}", ex);
            }
        }

        private void LoadDataFromFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<SaveData>(json);

                items.Clear();
                foreach (var item in data.Items)
                {
                    items.Add(item);
                }

                dollarStore = data.DollarStore;
                globalStrategy = data.GlobalStrategy;

                // Only update the season if the property exists in the saved data
                // This handles backward compatibility with old save files
                if (data.GetType().GetProperty("CurrentSeason") != null)
                {
                    currentSeason = data.CurrentSeason;
                    if (seasonCombo != null)
                        seasonCombo.SelectedIndex = (int)currentSeason;
                }

                // Update reasonable config if it exists
                if (data.GetType().GetProperty("ReasonableConfig") != null && data.ReasonableConfig != null)
                {
                    calculator.ReasonableConfig = data.ReasonableConfig;
                }

                UpdateCalculator();
                for (int i = 0; i < items.Count; i++)
                    UpdateGridAtIndex(i);
                SaveData();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading data: {ex.Message}", ex);
            }
        }

        private void SaveData()
        {
            try
            {
                SaveDataToFile(STORAGE_FILE);
                ShowStatusMessage("Data saved successfully");
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Error saving data: {ex.Message}", true);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(STORAGE_FILE))
                {
                    LoadDataFromFile(STORAGE_FILE);
                }
                else
                {
                    AddRow();
                }
                SaveData();
            }
            catch (Exception)
            {
                AddRow();
            }
        }

        private void ExportData(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json",
                FileName = "pricing-data.json"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SaveDataToFile(dialog.FileName);
                        MessageBox.Show("Data exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ImportData(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        LoadDataFromFile(dialog.FileName);
                        MessageBox.Show("Data imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ClearAllData(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear all data? This cannot be undone.",
                "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                items.Clear();
                AddRow();
                dollarStore = false;
                globalStrategy = EnumPricingStrategy.None;
                for (int i = 0; i < items.Count; i++) UpdateGridAtIndex(i);
            }
        }

        private void ShowStatusMessage(string message, bool isError = false)
        {
            statusLabel.Text = message;
            statusLabel.ForeColor = isError ? Color.Red : SystemColors.ControlText;

            // Auto-clear after 5 seconds if not an error
            if (!isError)
            {
                var timer = new Timer();
                timer.Interval = 5000;
                timer.Tick += (s, e) =>
                {
                    statusLabel.Text = "Ready";
                    statusLabel.ForeColor = SystemColors.ControlText;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }

        /// <summary>
        /// Gets a description of how the current season affects pricing
        /// </summary>
        private string GetSeasonImpactDescription()
        {
            switch (currentSeason)
            {
                case EnumSeason.Winter:
                    return "Winter pricing: 5% discount applied to stimulate sales during slower season.";
                case EnumSeason.Spring:
                    return "Spring pricing: 3% increase applied for new season merchandise.";
                case EnumSeason.Summer:
                    return "Summer pricing: 5% increase applied for peak season demand.";
                case EnumSeason.Fall:
                    return "Fall pricing: 2% increase applied for transitional season.";
                case EnumSeason.Holiday:
                    return "Holiday pricing: 8% increase applied for high-demand holiday season.";
                default:
                    return "Standard pricing: No seasonal adjustment applied.";
            }
        }

        /// <summary>
        /// Shows a tooltip with seasonal impact information when hovering over the season dropdown
        /// </summary>
        private void SetupSeasonTooltip()
        {
            var toolTip = new ToolTip();
            toolTip.SetToolTip(seasonCombo, "Select the current season to adjust pricing strategies.\n" +
                "Different seasons can impact product demand and pricing sensitivity.");

            // Also add a tooltip for the season label
            toolTip.SetToolTip(seasonLabel, GetSeasonImpactDescription());

            // Update the tooltip when the season changes
            seasonCombo.SelectedIndexChanged += (s, e) =>
            {
                toolTip.SetToolTip(seasonLabel, GetSeasonImpactDescription());
            };
        }

        /// <summary>
        /// Auto-detect season based on current date
        /// </summary>
        private EnumSeason DetectCurrentSeason()
        {
            var now = DateTime.Now;

            // Check if it's holiday season (November 15 - December 31)
            if ((now.Month == 11 && now.Day >= 15) || now.Month == 12)
                return EnumSeason.Holiday;

            // Otherwise determine by meteorological seasons
            switch (now.Month)
            {
                case 12:
                case 1:
                case 2:
                    return EnumSeason.Winter;
                case 3:
                case 4:
                case 5:
                    return EnumSeason.Spring;
                case 6:
                case 7:
                case 8:
                    return EnumSeason.Summer;
                case 9:
                case 10:
                case 11:
                    return EnumSeason.Fall;
                default:
                    return EnumSeason.Spring; // Fallback
            }
        }

        private void InitializeSeasonSettings()
        {
            // Auto-detect current season
            currentSeason = DetectCurrentSeason();
            seasonCombo.SelectedIndex = (int)currentSeason;

            UpdateCalculator();

            // Set up tooltips
            SetupSeasonTooltip();

            // Add seasonal impact information to status bar
            ShowStatusMessage(GetSeasonImpactDescription());
        }

    }
}