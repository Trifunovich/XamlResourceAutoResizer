using System;
using System.Collections;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xaml;
using System.Xml;
using XamlReader = System.Windows.Markup.XamlReader;

namespace XamlResourceAutoResizer.Helpers
{
  public static class XamlReaderHelper
  {
    private const string ResDict = "<ResourceDictionary ";

    private static string indent()
    {
      StringBuilder sb = new StringBuilder();
      foreach (var x in ResDict)
      {
        sb.Append(" ");
      }

      return sb.ToString();
    }

    public static void ReadXamlFromFile(string path)
    {
      var rd = new ResourceDictionary();
      //rd.Source = new Uri(@"Resource.xaml", UriKind.Relative);
      object key = string.Empty;
      object value;
      var _lineIndent = indent();
      string smallIndent = _lineIndent.Remove(4);
      using (FileStream stream = new FileStream(path, FileMode.Open))
      {
        object reader = null;
        try
        {
          reader = XamlReader.Load(stream);
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }

        string stripDownFile = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal))
          .Replace(".xaml", string.Empty);
        if (reader != null)
        {
          ResourceDictionary content = reader as ResourceDictionary;
         
          if (content != null)
          {
            StringBuilder beginningTag = new StringBuilder();
            beginningTag.AppendLine(ResDict + "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            beginningTag.AppendLine(_lineIndent + "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            beginningTag.AppendLine(_lineIndent + "xmlns:core=\"clr-namespace:System;assembly=mscorlib\"> ");
            int index = 0;
            foreach (var x in content)
            {
              if (!(x is DictionaryEntry))
              {
                continue;
              }

              DictionaryEntry entry = (DictionaryEntry)x;
              double pg = entry.Value is double ? (double) entry.Value : 0;
              if (pg != 0)
              {
                value = entry.Value;
                key = entry.Key;

                beginningTag.AppendLine(smallIndent + $"<core:Double x:Key=\"{key}\">{value}</system:String>");
              }
            }
            beginningTag.AppendLine("</ResourceDictionary>");
            System.IO.File.WriteAllText(@"Resource.xaml", beginningTag.ToString());
          }
        }

        //Write to xaml file
        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
        //sb.AppendLine("    <ResourceDictionary.MergedDictionaries>");
        //sb.AppendLine(files.ToString());
        //sb.AppendLine("    </ResourceDictionary.MergedDictionaries>");
        //sb.AppendLine("</ResourceDictionary>");
      
      }

     
    }
  }
}