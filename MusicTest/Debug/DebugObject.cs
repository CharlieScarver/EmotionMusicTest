using Emotion.Graphics;
using Emotion.Primitives;
using MusicTest.GameObjects;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;
using Emotion.Utility;

namespace MusicTest.Debug
{
    public class DebugObject
    {

        public DebugObject(GameObject item)
        {
            this.Item = item;
            this.Attributes = new List<PropertyInfo>(item.GetType().GetProperties());
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

        public List<PropertyInfo> Attributes { get; set; }

        ///<summary>Returns each property's name, value and type as a string for the item passed in the ctor</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.Item.GetType().Name);
            sb.AppendLine(this.Item.Name);
            foreach (PropertyInfo prop in Attributes)
            {
                if (prop.Name != "ModelMatrix") // Excluded long prop/s
                {
                    sb.AppendLine(string.Format("{0} = {1}   ({2})", prop.Name, prop.GetValue(this.Item), prop.PropertyType.Name));
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

       ///<summary>
       /// Render the object's Transform rectangle.
       /// Emotion specific method.
       ///</summary>
       public void RenderObjectRectange(RenderComposer composer)
        {
            // Check if Item can be cast to Unit in "unit" variable and if so provides the "unit" variable for further use
            if (Item is Unit unit)
            {
                // Draw texture center
                composer.RenderCircle(Item.Center.ToVec3(Item.Z), 3, Color.Green, true);

                if (unit.InclineAngle != 0f)
                {
                    composer.PushModelMatrix(
                        Matrix4x4.CreateRotationZ(unit.InclineAngle, new Vector3(unit.Center, 0))
                    );
                }
                // Draw the texture rectangle
                composer.RenderOutline(Item.Position, Item.Size, Color.Green, 2);

                if (unit.InclineAngle != 0f)
                {
                    composer.PopModelMatrix();
                }

                // Draw future position for units
                Vector3 futurePosition = new Vector3(
                    unit.CollisionBox.X,
                    unit.CollisionBox.Y - (unit.GravityTimer.Progress * unit.StartingVelocityY),
                    unit.CollisionBox.Z
                );
                composer.RenderOutline(futurePosition, unit.CollisionBox.Size, Color.Cyan, 1);
                // Draw CollisionBox for units
                composer.RenderOutline(unit.CollisionBox.Position, unit.CollisionBox.Size, Color.Red, 2);
            }
            else if (Item is Decoration dec)
            {
                // Draw texture center
                composer.RenderCircle(Item.Center.ToVec3(Item.Z), 3, Color.Green, true);

                // Draw CollisionBox for units
                composer.RenderOutline(dec.Position, dec.DisplaySize, Color.Red, 2);
            }
        }
    }
}
