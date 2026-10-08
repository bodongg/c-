using System.Drawing;
using System.Windows.Forms;

namespace DriveQuest.Forms;

internal static class Ui
{
    public static readonly Color Background = Color.FromArgb(14, 20, 33);
    public static readonly Color Surface = Color.FromArgb(25, 34, 51);
    public static readonly Color Surface2 = Color.FromArgb(35, 47, 68);
    public static readonly Color Text = Color.FromArgb(236, 242, 249);
    public static readonly Color Muted = Color.FromArgb(153, 170, 192);
    public static readonly Color Blue = Color.FromArgb(57, 130, 247);
    public static readonly Color Green = Color.FromArgb(46, 190, 134);
    public static readonly Color Red = Color.FromArgb(235, 96, 104);
    public static readonly Color Gold = Color.FromArgb(245, 190, 69);

    public static void StyleForm(Form form, string title, int width = 1100, int height = 730)
    {
        form.Text = title;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.Size = new Size(width, height);
        form.MinimumSize = new Size(850, 600);
        form.BackColor = Background;
        form.ForeColor = Text;
        form.Font = new Font("Segoe UI", 10);
    }

    public static Label Label(string text, int size = 10, bool bold = false, Color? color = null)
        => new()
        {
            Text = text, ForeColor = color ?? Text, AutoSize = true,
            Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
            Margin = new Padding(0, 0, 0, 8)
        };

    public static Button Button(string text, Color? color = null)
        => new()
        {
            Text = text, Height = 42, Width = 135,
            BackColor = color ?? Blue, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Margin = new Padding(0, 0, 9, 0)
        };

    public static TextBox Input(int width = 180)
        => new()
        {
            Width = width, Height = 34, BackColor = Surface2, ForeColor = Text,
            BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0, 0, 14, 12)
        };

    public static ComboBox Combo(int width = 180)
        => new()
        {
            Width = width, Height = 34, DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Surface2, ForeColor = Text, FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0, 0, 14, 12)
        };

    public static DataGridView Grid()
    {
        DataGridView grid = new()
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            AllowUserToDeleteRows = false, MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Surface, BorderStyle = BorderStyle.None,
            RowHeadersVisible = false, EnableHeadersVisualStyles = false,
            GridColor = Surface2, ColumnHeadersHeight = 40,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            RowTemplate = { Height = 36 }
        };
        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor = Blue;
        grid.DefaultCellStyle.SelectionForeColor = Color.White;
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
        grid.ColumnHeadersDefaultCellStyle.BackColor = Surface2;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        return grid;
    }

    public static FlowLayoutPanel Row()
        => new() { Dock = DockStyle.Top, Height = 54, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };

    public static void Error(Exception ex) => MessageBox.Show(ex.Message, "Drive Quest", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
