using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DEVGIS.CsharpLibs
{
    /// <summary>
    /// 
    /// </summary>
    public static class TextEditExtend
    {
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool NotEmpty(this TextBox t)
        {
            return string.IsNullOrEmpty(t.Text.Trim());
        }

        /// <summary>
        /// 输入的是数字
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNumber(this TextBox t)
        {
            try
            {
                int value = Convert.ToInt32(t.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 输入的是小数
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsToDecimal(this TextBox t)
        {
            try
            {
                decimal value = Convert.ToDecimal(t.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 输入的是min~max之间的数字(包含两边)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNumber(this TextBox t,int min,int max)
        {
            try
            {
                int value = Convert.ToInt32(t.Text);
                if (value <= max && value >= min)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
