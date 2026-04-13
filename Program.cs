using Planetarium_Urania.Forms;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

using var loginForm = new LoginForm();
if (loginForm.ShowDialog() != DialogResult.OK)
    return;

Application.Run(new MainForm());