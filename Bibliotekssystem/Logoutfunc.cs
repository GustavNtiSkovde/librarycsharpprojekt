using System.Windows;

namespace Bibliotekssystem {
    public static class LogoutManager {
        /// <summary>
        /// Logs out the user by resetting the MainWindow content back to the default login screen.
        /// </summary>
        public static void Logout() {
            // Get the main application window
            if (Application.Current.MainWindow is MainWindow mainWindow) {
                // Clear the content frame/view and replace with initial state
                // If MainWindow initializes components needed for login, recreate or reset its view
                mainWindow.Content = null; // or restore default Login control/view

                // Alternatively, open a brand-new MainWindow instance and close the old one
                var newMainWindow = new MainWindow();
                Application.Current.MainWindow = newMainWindow;
                newMainWindow.Show();

                mainWindow.Close();
            }
        }
    }
}