using System.Collections.Generic;
using System.Text;
using XamlResourceAutoResizer.Helpers;

namespace XamlResourceAutoResizer.ViewModel
{
  public class MainWindowViewModel
  {
    public Dictionary<int, bool> SizeCheckDictionary { get; set; }


    public MainWindowViewModel()
    {
      SizeCheckDictionary = new Dictionary<int, bool>();
      for (int i = 25; i <= 200; i += 25)
      {
        SizeCheckDictionary[i] = true;
      }
    }

    public IEnumerable<IResourceDisplayModel> PopulateResults(string filePath, bool createFiles)
    {
      var values = ReadXamlValues(filePath);
      return values;
    }

    private IEnumerable<IResourceDisplayModel> ReadXamlValues(string path)
    {
      var results = XamlReaderHelper.ReadXamlFromFile(@"C:\Users\alt\Documents\BEC\UI\EvidenceCenter.Ui.Localization\XamlResources\04-Design\03-ComplexStyles\00-ButtonStyles.txaml");
      //ResultingText.Append(results);
      return results;
    }

  }
}