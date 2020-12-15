using System.Collections.Generic;
using System.Text;
using XamlResourceAutoResizer.Helpers;

namespace XamlResourceAutoResizer.ViewModel
{
  public class MainWindowViewModel
  {
    public Dictionary<int, bool> SizeCheckDictionary { get; set; }
    public StringBuilder ResultingText { get; private set; }

    public MainWindowViewModel()
    {
      SizeCheckDictionary = new Dictionary<int, bool>();
      for (int i = 25; i <= 200; i += 25)
      {
        SizeCheckDictionary[i] = true;
      }
    }

    public void PopulateResults(string filePath, bool createFiles)
    {
      ResultingText = new StringBuilder();
      var values = ReadXamlValues(filePath);
      //foreach (KeyValuePair<int, bool> size in SizeCheckDictionary)
      //{
      //  if (size.Value)
      //  {
      //    foreach (var val in values)
      //    {
      //      ResultingText.Append(val * size.Key / 100);
      //    }

      //    ResultingText.AppendLine();
      //  }
      //}
    }

    private IEnumerable<double> ReadXamlValues(string path)
    {
      XamlReaderHelper.ReadXamlFromFile(path);
      return null;
    }

  }
}