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
using Microsoft.Win32;  // Jmenný prostor pro možnost uložení dat do souboru

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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře ImportDatWindow.xaml
   /// Třída slouží k nastavení okna pro načtení dat ze souboru.
   /// </summary>
   public partial class ImportDatWindow : Window
   {
      /// <summary>
      /// Instance hlavního okna pro možnost využití funkcí hlavního okna při práci s vybraným záznamem
      /// </summary>
      private MainWindow HlavniOkno;

      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;



      /// <summary>
      /// Konstruktor třídy pro import dat ze souboru
      /// </summary>
      /// <param name="spravce">Instance správce aplikace</param>
      /// <param name="HlavniOkno">Instance hlavního okna</param>
      public ImportDatWindow(Spravce_Aplikace spravce, MainWindow HlavniOkno)
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Náhradí parametrů konstruktoru do interních proměnných
         this.SpravaAplikace = spravce;
         this.HlavniOkno = HlavniOkno;

         // Nastavení barvy pozadí
         Background = HlavniOkno.BarvaPozadi;
      }



      /// <summary>
      /// Metoda pro načtení dat ze souboru
      /// </summary>
      /// <param name="Prepsat">0 - záznamy budou přidány do stávající kolekce, 1 - původní záznamy budou smazány a přidají se nové záznamy</param>
      private void UlozeniZaznamu(byte Prepsat)
      {
         try
         {
            // Vytvoření funkce pro načtení dat ze souboru s nastavením nabídky formátů pro uložení
            OpenFileDialog dialog = new OpenFileDialog
            {
               Filter = "Data files (*.xml)|*.xml|All files (*.*)|*.*"
            };

            // Zobrazení dialogu pro vybrání cesty k uložneí s návratovou hodnotou určující zda bylo uložení potvrzeno
            if (dialog.ShowDialog() == true)
            {
               // Kontrola zda byl zadán název souboru pro uložení
               if (string.IsNullOrWhiteSpace(dialog.FileName))
                  throw new ArgumentException("Zadejte název souboru!");

               // Volání metody pro načtení dat ze souboru
               SpravaAplikace.NactiData_XML(dialog.FileName, Prepsat);

               // Zavření okna pro import dat
               Close();
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Obsluha tlačítko pro přidání novách záznamů importovaných ze souboru do kolekce stávajících záznamů přihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PridatNoveZaznamyButton_Click(object sender, RoutedEventArgs e)
      {
         UlozeniZaznamu(0);
      }

      /// <summary>
      /// Obsluha tlačítko pro přepsání stávajících záznamů záznamy importovanými ze souboru.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void AktualizovatZaznamyButton_Click(object sender, RoutedEventArgs e)
      {
         // Zobrazení upozornění s uložením zvolené volby
         MessageBoxResult VybranaMoznost = MessageBox.Show("Původní záznamy budou smazány a budou nahrazeny novými! \n\t\tPřejete si pokračovat?", "Pozor!", MessageBoxButton.YesNo, MessageBoxImage.Question);

         // Reakce na stisknuté tlačítko
         switch (VybranaMoznost)
         {
            case MessageBoxResult.Yes: UlozeniZaznamu(1);   break;
            case MessageBoxResult.No:                       break;
            default: break;
         }
      }

   }
}
