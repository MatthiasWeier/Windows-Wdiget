using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.WinForms;

namespace Windows_Gadget;

public partial class OverlayForm : Form {
  private readonly int _OVERLAY_WIDTH = Settings1.Default.OverlayWidth;
  private readonly int _OVERLAY_LENGTH = Settings1.Default.OverlayLength;
  private readonly int _OVERLAY_X = Settings1.Default.OverlayX;
  private readonly int _OVERLAY_Y = Settings1.Default.OverlayY;
  private readonly Uri _WEBSITE = new(Settings1.Default.WebsiteToShow);
  private static class NativeMethods {
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_LAYERED = 0x80000;
    public const int WS_EX_TRANSPARENT = 0x20;

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
  }

  public OverlayForm() {
    this.InitializeComponent();

    this.FormBorderStyle = FormBorderStyle.None;
    this.TopMost = true;

    this.Size = new(this._OVERLAY_WIDTH, this._OVERLAY_LENGTH);
    this.StartPosition = FormStartPosition.Manual;
    this.ShowInTaskbar = false;

    if (Screen.PrimaryScreen != null)
      this.Location = new(Screen.PrimaryScreen.WorkingArea.Width + this._OVERLAY_X, this._OVERLAY_Y);

    this.InitWebView();
    this.MakeClickThrough();
  }


  private void InitWebView() {
    var webView = new WebView2 {
      Dock = DockStyle.Fill
    };

    this.Controls.Add(webView);
    webView.Source = this._WEBSITE;

    webView.NavigationCompleted += (s, e) => { webView.ExecuteScriptAsync("document.body.style.overflow = 'hidden';"); };
    webView.DefaultBackgroundColor = Color.Transparent;
  }
  private void MakeClickThrough() {
    var exStyle = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_EXSTYLE);
    exStyle |= NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT;
    var windowLong = NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL_EXSTYLE, exStyle);
  }

  private void OverlayForm_Load(object sender, EventArgs e) { }
}
