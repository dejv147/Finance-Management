using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

/// <summary>
/// Aplikace pro správu financí určena pouze pro osobní užití.
/// Aplikace pracuje vždy pouze s daty patřící přihlášenému uživateli, 
/// avšak do souboru ukládá a při spuštění načítá veškerá data pro zpracování, tedy data všech registrovaných uživatelů.
/// V hlavním okně aplikace je zobrazen stručný přehled a je zde uživatelské rozhraní pro správu aplikace i pro možnost využití dalších funkcí aplikace pracujících v samostatných oknech.
/// 
/// Autor projektu: bc. David Halas
/// Link uložení projektu: https://github.com/dejv147/Finance-Management
/// </summary>
namespace Sprava_financi
{
   /// <summary>
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře InformacniWindow.xaml
   /// Třída slouží pro otevření okna s výpisem základních informací o aplikaci. Výpis obsahuje stručný popis všech funkcí a návod pro obsluhu aplikace.
   /// </summary>
   public partial class InformacniWindow : Window
   {
      /// <summary>
      /// Textový návod k aplikaci
      /// </summary>
      private string Popis;


      /// <summary>
      /// Konstruktor třídy
      /// </summary>
      /// <param name="Pozadi">Barva pozadí</param>
      public InformacniWindow(Brush Pozadi)
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Inicializace textového řetězce
         Popis = "";

         // Nastavení barvy pozadí
         Background = Pozadi;

         // Vypsání textového řetězce
         VypisNavod();
      }


      /// <summary>
      /// Metoda pro vytvoření textového návodu k aplikaci a zobrazení textového řetězce do informačního okna
      /// </summary>
      private void VypisNavod()
      {
         Nazev.Text = "Aplikace pro správu financí konkrétního uživatele.";

         Popis = "Každý registrovaný uživatel má vlastní kolekci dat, nelze tedy zobrazovat data jiného uživatele.\n";
         Popis += "V hlavním okně aplikace jsou zobrazeny poslední přidané záznamy včetně měsíčního přehledu.\n\n";

         Popis += "V levé části je k dispozici rozbalovací MENU pro práci se záznamy v aplikaci pro přihlášeného uživatele.\n";
         Popis += "V pravé části je k dispozici rozbalovací MENU pro správu funkcí uživatele. \n";
         Popis += "Je zde k dispozici jednoduchá kalulačka, možnost načtení a uložení dat do souboru, \nmožnost změnit barvu pozadí, zobrazení poznámkového bloku pro uživatele \na tlačítko pro odhlášení aktuálního uživatele.\n\n";

         Popis += "Aplikace zpracovává a uchovává kolekci jednotlivých záznamů, kde každý záznam představuje 1 finanční transakci.\n";
         Popis += "Uživatel má možnost záznamy přidávat (vytvářet), upravovat a odebírat (smazat). \n\n";

         Popis += "Každý záznam obsahuje základní údaje s možností přidat poznámku ke každému záznamu, \nvčetně možnosti přidat seznam položek k danému záznamu (účtence).\n";
         Popis += "Je možné zobrazit vybrané záznamy v samostatném seznamu, \ndále je možnost vyhledat určité záznamy dle zadaných filtrů vyhledávání.\n\n";

         Popis += "Je možné exportovat vybrané záznamy a uložit je do textového souboru (*.txt), \nseparovaného textového souboru (*.csv), nebo do datového souboru (*.xml).\n";
         Popis += "Záznamy uložené ve formátu XML je možné importovat do aplikace, \nkde je možnost zvolit přidání novách záznamů mezi existující, nebo nahradit stávající záznamy novými.\n\n";

         Popis += "Dále možnost zobrazit statisticky vyhodnocené údaje v podobě grafů. \nJe k dispozici sloupcový graf shrnující příjmy i výdaje v určitých časových intervalech.\n";
         Popis += "Hodnoty časové osy jsou nastavitelné dle požadovaného zobrazení.\n\n";
         Popis += "Druhým grafem je zobrazení příjmů a výdajů v zadaném časovém intervalu rozděleném na jednotlivé kategorie.\n\n";
         Popis += "V obou grafech je možné se pohybovat v rámci časové osy,\n nebo mezi jednotlivými kategoriemi pomocí červených šipek v právém spodním rohu grafu.\n";
         Popis += "Mezi jednotlivými grafy se lze přepínat pomocí modrých šipek v pravém horním rohu.\n\n";


         // Vypsání textového řetězce do informační okna 
         NavodLabel.Content = Popis;
      }

   }
}
