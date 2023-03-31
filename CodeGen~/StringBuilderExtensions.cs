using System.Text;

namespace UnityInjectorCodeGen {
    public static class StringBuilderExtensions {
        public static void Append(this StringBuilder stringBuilder, string str) {
            stringBuilder.Append(str);
        }
    
        public static void Append(this StringBuilder stringBuilder, string str, string str1) {
            stringBuilder.Append(str);
            stringBuilder.Append(str1);
        }
    
        public static void Append(this StringBuilder stringBuilder, string str, string str1, string str2) {
            stringBuilder.Append(str);
            stringBuilder.Append(str1);
            stringBuilder.Append(str2);
        }
    
        public static void Append(this StringBuilder stringBuilder, string str, string str1, string str2, string str3) {
            stringBuilder.Append(str);
            stringBuilder.Append(str1);
            stringBuilder.Append(str2);
            stringBuilder.Append(str3);
        }
    
        public static void Append(this StringBuilder stringBuilder, string str, string str1, string str2, string str3, string str4) {
            stringBuilder.Append(str);
            stringBuilder.Append(str1);
            stringBuilder.Append(str2);
            stringBuilder.Append(str3);
            stringBuilder.Append(str4);
        }
    }
}
