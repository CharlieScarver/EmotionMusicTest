using MusicTest.GameObjects;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MusicTest.Debug
{
    public class DebugObject
    {

        public DebugObject(GameObject item)
        {
            this.Item = item;
            this.Attributes = new List<System.Reflection.PropertyInfo>(item.GetType().GetProperties());
        }

        ///<summary>Get the longest line in the debug text so the next anchor can be properly indented</summary>
        public string LongestLine
        {
            get
            {
                int max = 0;
                string longest = "";
                foreach (var item in Attributes)
                {
                    if (item.Name != "ModelMatrix") // Excluded long prop/s
                    {
                        string text = string.Format("{0} = {1}   ({2})", item.Name, item.GetValue(this.Item, null), item.GetType().Name);
                        if (text.Length > max)
                        {
                            max = text.Length;
                            longest = text;
                        }
                    }
                }
                return longest;
            }
        }
        
        public GameObject Item { get; set; }

        public Vector2 Position { get; set; }

        public List<System.Reflection.PropertyInfo> Attributes { get; set; }

        ///<summary>Returns each property's name, value and type as a string for the item passed in the ctor</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.Item.GetType().Name);
            sb.AppendLine(this.Item.Name);
            foreach (var item in Attributes)
            {
                if (item.Name != "ModelMatrix") // Excluded long prop/s
                {
                    sb.AppendLine(string.Format("{0} = {1}   ({2})", item.Name, item.GetValue(this.Item, null), item.PropertyType.Name));
                }
            }
            return sb.ToString();
        }

        ///<summary>Returns each property's name and value for the object passed</summary>
        /// <param name="obj">The object passed</param>
        public static string ToString(IGameObject obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(obj.GetType().Name);
            sb.AppendLine(obj.ToString());
            foreach (var prop in obj.GetType().GetProperties())
            {
                sb.AppendLine(string.Format("{0} = {1}", prop.Name, prop.GetValue(obj, null)));
            }
            return sb.ToString();
        }
    }
}
