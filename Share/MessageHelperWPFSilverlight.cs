using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DEVGIS.CsharpLibs
{
    public class MessageHelper
    {
        private static string MessageTitle = "CsharpLibs";

        /// <summary>
        /// Show a error message
        /// </summary>
        /// <param name="Error"></param>
        public static void ShowError(string Error)
        {
            MessageBox.Show(Error, MessageTitle,MessageBoxButton.OK,MessageBoxImage.Error);
        }

        /// <summary>
        /// Show a info message
        /// </summary>
        /// <param name="Info"></param>
        public static void ShowInfo(string Info)
        {
            MessageBox.Show(Info, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Show a question when click 'Yes' Button return ture else return false
        /// </summary>
        /// <param name="Question"></param>
        /// <returns></returns>
        public static bool ShowQuestion(string Question)
        {
            if (MessageBox.Show(Question, MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
            return false;
        }
    }
}
