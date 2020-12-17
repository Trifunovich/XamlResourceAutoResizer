using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Accessibility;
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

    public static IEnumerable<IResourceDisplayModel> ReadXamlFromFile(string otherResource)
    {
      ResourceDictionary sizesContent =
        GetResourceDictionary(@"C:\Users\alt\Documents\BEC\UI\EvidenceCenter.Ui.Localization\Design\FontsMarginsAndSizes.xaml");
      string otherContent = DictContent(otherResource);
      return AllIndexesOf(otherContent, @"Value=""", otherResource, sizesContent);

     

      //var rd = new ResourceDictionary();
      //object key = string.Empty;
      //object value;
      //var _lineIndent = indent();
      //string smallIndent = _lineIndent.Remove(4);
      //ResourceDictionary colorsDict =
      //  GetResourceDictionary(@"C:\Users\alt\Documents\BEC\UI\EvidenceCenter.Ui.Localization\DesignPresets\Default.xaml");
      //Application.Current.Resources.MergedDictionaries.Add(colorsDict);
      //ResourceDictionary brushesDict =
      //  GetResourceDictionary(@"C:\Users\alt\Documents\BEC\UI\EvidenceCenter.Ui.Localization\XamlResources\02-Base\01-Brushes.txaml");
      //Application.Current.Resources.MergedDictionaries.Add(brushesDict);


      //DirectorySearch(@"C:\Users\alt\Documents\BEC\UI\EvidenceCenter.Ui.Localization\XamlResources\03-Icons");

      //ResourceDictionary tempDict =
      //  GetResourceDictionary(@"C:\Users\alt\Documents\BEC\UI\EvidenceCenter.Ui.Localization\XamlResources\04-Design\02-Templates\00-ButtonTemplates.txaml");
      //Application.Current.Resources.MergedDictionaries.Add(tempDict);
      //ResourceDictionary sizeDict = GetResourceDictionary(@"C:\Users\alt\Documents\BEC\UI\EvidenceCenter.Ui.Localization\Design\FontsMarginsAndSizes.xaml");
      //Application.Current.Resources.MergedDictionaries.Add(sizeDict);
      //ResourceDictionary pathsDict = GetResourceDictionary(@"C:\Users\alt\Documents\BEC\UI\EvidenceCenter.Ui.Localization\XamlResources\04-Design\01-Styles\02-PathStyles.txaml");
      //Application.Current.Resources.MergedDictionaries.Add(pathsDict);
      //ResourceDictionary otherDict = GetResourceDictionary(otherResource);



      //Write to xaml file
      //StringBuilder sb = new StringBuilder();
      //sb.AppendLine("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
      //sb.AppendLine("    <ResourceDictionary.MergedDictionaries>");
      //sb.AppendLine(files.ToString());
      //sb.AppendLine("    </ResourceDictionary.MergedDictionaries>");
      //sb.AppendLine("</ResourceDictionary>");


      //if (sizeDict != null && otherDict != null)
      //{
      //  StringBuilder beginningTag = new StringBuilder();
      //  beginningTag.AppendLine(ResDict + "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
      //  beginningTag.AppendLine(_lineIndent + "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
      //  beginningTag.AppendLine(_lineIndent + "xmlns:core=\"clr-namespace:System;assembly=mscorlib\"> ");
      //  foreach (var x in otherDict)
      //  {
      //    if (!(x is DictionaryEntry))
      //    {
      //      continue;
      //    }

      //    DictionaryEntry entry = (DictionaryEntry)x;
      //    double pg = entry.Value is double ? (double)entry.Value : 0;
      //    if (pg != 0)
      //    {
      //      value = entry.Value;
      //      key = entry.Key;

      //      beginningTag.AppendLine(smallIndent + $"<core:Double x:Key=\"{key}\">{value}</system:String>");
      //    }
      //  }
      //  beginningTag.AppendLine("</ResourceDictionary>");
      //  System.IO.File.WriteAllText($"SomeOther.xaml", beginningTag.ToString());

      //}

    }
    private static Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
    private static Regex regex2 = new Regex(@"[0-9 ]+");
    private static Regex oneCharOneNum = new Regex(@".*[0-9].*");
    public static IEnumerable<IResourceDisplayModel> AllIndexesOf(string oldstr, string substr, string file, ResourceDictionary sizeDict, bool ignoreCase = false)
    {
      string str = oldstr;
      if (string.IsNullOrWhiteSpace(str) ||
          string.IsNullOrWhiteSpace(substr))
      {
        throw new ArgumentException("String or substring is not specified.");
      }

      var definedSizes = sizeDict.Cast<DictionaryEntry>().ToList();

      var indexes = new List<ResourceDisplayModel>();
      int index = 0;

      while ((index = str.IndexOf(substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
      {
        var keySubstring = str.Substring(0, index);
        string propString = @"Property=""";
        char quote = '"';
        var keyIndex = keySubstring.LastIndexOf(propString) + propString.Length;
        var keyStr = keySubstring.Substring(keyIndex, keySubstring.LastIndexOf(quote) - keyIndex);
        str = str.Remove(0, index);
        str = str.Remove(0, str.IndexOf(quote) + 1);
        index = 0;
        var splitted = str.Substring(index, str.IndexOf('"') - index);
        if (!splitted.Contains("StaticResource") 
            && oneCharOneNum.IsMatch(splitted)
            && !string.IsNullOrWhiteSpace(splitted)
            && !splitted.Contains("DynamicResource")
            && !splitted.Contains("Binding")
            && ((splitted.Contains(",") && CheckIfPartsNum(splitted)) || regex.IsMatch(splitted) || (regex2.IsMatch(splitted) && CheckIfPartsNum(splitted, false))))
        {
          var keyMatch = string.Empty;
          bool isThickness = keyStr.Contains("Thickness") || keyStr.Contains("Margin") || keyStr.Contains("Padding");
          object value;

          if (isThickness)
          {
            value = ConvertStringToThickness(splitted, splitted.Contains(","));
          }
          else
          {
            value = double.Parse(splitted);
          }

         
          if (definedSizes.Any(s => s.Value.Equals(value)))
          {
            var first = definedSizes.FirstOrDefault(s => s.Value.Equals(value));
            keyMatch = first.Key.ToString();
          }
          if (isThickness)
          {
            indexes.Add(new ThicknessDisplayModel(keyStr,file, keyMatch, (Thickness)value));
          }
          else
          {
            indexes.Add(new DoubleDisplayModel(keyStr, file, keyMatch, (double)value));
          }
         
        }
      }

      return indexes;
    }

    private static bool CheckIfPartsNum(string str, bool withComma = true)
    {
      List<bool> results = new List<bool>();
      char sep = withComma ? ',' : ' ';
      var parts = str.Split(sep);
      foreach (var part in parts)
      {
        results.Add(regex.IsMatch(part));
      }
      return results.All(s => s);
    }

    private static Thickness ConvertStringToThickness(string str, bool withComma)
    {
      Thickness thick = new Thickness();
      char sep = withComma ? ',' : ' ';
      var parts = str.Split(sep);
      int count = parts.Length;
      var filtered = parts.Where(x => regex.IsMatch(x)).ToList();
      foreach (var part in filtered)
      {
        double val = double.Parse(part);
        int index = filtered.IndexOf(part);
        switch (count)
        {
          case 1:
            thick = new Thickness(val);
            break;
          case 2:
            if (index == 0)
            {
              thick.Left = thick.Right = val;
            }
            else
            {
              thick.Top = thick.Bottom = val;
            }
            break;
          case 4:
          {
            switch (index)
            {
                case 1:
                  thick.Left = val;
                  break;
                case 2:
                  thick.Top  = val;
                  break;
                case 3:
                  thick.Right = val;
                  break;
                case 4:
                  thick.Bottom = val;
                  break;
            }
            break;
          }
        }

      }
      return thick;
    }

    private static void DirectorySearch(string dir)
    {
      try
      {
        foreach (string f in Directory.GetFiles(dir))
        {
          ResourceDictionary sizeDict = GetResourceDictionary(f);
          Application.Current.Resources.MergedDictionaries.Add(sizeDict);
        }

        foreach (string d in Directory.GetDirectories(dir))
        {
          Console.WriteLine(Path.GetFileName(d));
          DirectorySearch(d);
        }
      }
      catch (System.Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private static ResourceDictionary GetResourceDictionary(string path)
    {
      using (FileStream stream = new FileStream(path, FileMode.Open))
      {
        object reader = null;
        try
        {
          reader = XamlReader.Load(stream);
        }
        catch (Exception e)
        {
          string contents;
          stream.Position = 0;
          using (var sr = new StreamReader(stream))
          {
            contents = sr.ReadToEnd();
          }

          if (!string.IsNullOrEmpty(contents) && contents.Contains("<ImageSource"))
          {
            var begin = contents.IndexOf("<ImageSource");
            var endd = contents.LastIndexOf("</ImageSource>") + "</ImageSource>".Length;
            contents = contents.Remove(begin, endd - begin);
            reader = XamlReader.Parse(contents);
          }

        }

        return reader as ResourceDictionary;
      }
    }

    private static string DictContent(string path)
    {
      using (FileStream stream = new FileStream(path, FileMode.Open))
      {
        string contents;
        stream.Position = 0;
        using (var sr = new StreamReader(stream))
        {
          contents = sr.ReadToEnd();
        }

        return contents;
      }
    }
  }

  public interface IResourceDisplayModel
  {
    string Key { get; }
    string File { get; }
    string FMSKeyMatch { get; }
    
    bool Ignore { get; }

    bool IsMissingKey { get; }
  }

  public abstract class ResourceDisplayModel : IResourceDisplayModel
  {

    public ResourceDisplayModel(string key, string file, string fmsKeyMatch)
    {
      Key = key;
      File = file;
      FMSKeyMatch = fmsKeyMatch;
    }

    public string Key { get; }

    public string File { get; }

    public string FMSKeyMatch { get; }

    public abstract bool Ignore
    {
      get;
    }

    public bool IsMissingKey => string.IsNullOrEmpty(FMSKeyMatch);

    public bool IsThickness => Key.Contains("Thickness") || Key.Contains("Padding") || Key.Contains("Margin");

  }

  public class DoubleDisplayModel : ResourceDisplayModel
  {

    public DoubleDisplayModel(string key, string file, string fmsKeyMatch, double value) : base(key, file, fmsKeyMatch)
    {
      Value = value;
    }

    public double Value { get; }

    public override bool Ignore => Value == 0;
  }

  public class ThicknessDisplayModel : ResourceDisplayModel
  {
    public ThicknessDisplayModel(string key, string file, string fmsKeyMatch, Thickness value) : base(key, file, fmsKeyMatch)
    {
      Value = value;
    }

    public Thickness Value { get; }

    public override bool Ignore => Value.Right == 0 && Value.Left == 0 && Value.Top == 0 && Value.Bottom == 0;
  }
}