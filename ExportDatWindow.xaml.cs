using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře ExportDatWindow.xaml
   /// Třída slouží k nastavení okna pro možnost výběru dat a následné uložení vybraných do souboru včetně možnosti výběru formátu uložení.
   /// </summary>
   public partial class ExportDatWindow : Window
   {
      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;

      /// <summary>
      /// Kolekce dat určena k uložení do souboru
      /// </summary>
      public ObservableCollection<Zaznam> KolekceZaznamu { get; private set; }

      /// <summary>
      /// Kolekce dat určena k uložení do souboru
      /// </summary>
      public ObservableCollection<Zaznam> VybraneZaznamy { get; private set; }

      /// <summary>
      /// Instance třídy pro převedení kolekce záznamů do grafické podoby
      /// </summary>
      private GrafickyZaznam grafickyZaznam;

      /// <summary>
      /// Instance třídy pro vykreslování grafických prvků
      /// </summary>
      private Vykreslovani vykreslovani;

      /// <summary>
      /// Instance třídy pro vyhledávání konkrétních dat
      /// </summary>
      private Vyhledavani vyhledavani;

      /// <summary>
      /// Instance hlavního okna pro možnost využití funkcí hlavního okna při práci s vybraným záznamem
      /// </summary>
      private MainWindow HlavniOkno;



      /// <summary>
      /// Konstruktor třídy pro export dat do souboru
      /// </summary>
      /// <param name="spravce">Instance správce aplikace</param>
      /// <param name="KolekceZaznamu">Kolekce záznamů k uložení do souboru</param>
      /// <param name="HlavniOkno">Instance hlavního okna</param>
      public ExportDatWindow(Spravce_Aplikace spravce, ObservableCollection<Zaznam> KolekceZaznamu, MainWindow HlavniOkno)
      {
         // Inicializace
         InitializeComponent();
         this.SpravaAplikace = spravce;
         this.KolekceZaznamu = KolekceZaznamu;
         VybraneZaznamy = KolekceZaznamu;
         this.HlavniOkno = HlavniOkno;
         vykreslovani = new Vykreslovani();
         grafickyZaznam = new GrafickyZaznam(null, null);
         vyhledavani = new Vyhledavani();

         // Nastavení barvy pozadí
         Background = HlavniOkno.BarvaPozadi;

         // Úvodní vykreslení záznamů určených k uložení do souboru
         VykresliZaznamy();
      }



      /// <summary>
      /// Metoda pro vykreslení vybraných záznamů
      /// </summary>
      private void VykresliZaznamy()
      {
         // Smazání obsahu plátna pro možnost opětovného vykreslení
         SeznamZaznamuCanvas.Children.Clear();

         // Načtení kolekce dat ke grafickému zpracování
         grafickyZaznam.ObnovKolekciZaznamu(VybraneZaznamy);

         // Vykreslení zpracovaného seznamu záznamů do úvodního okna aplikace
         vykreslovani.VykresliPrvek(SeznamZaznamuCanvas, grafickyZaznam.StrankaZaznamu);
      }

      /// <summary>
      /// Načtení vybraných dat do interní kolekce určené k uložení do souboru
      /// </summary>
      /// <param name="VybranaKolekceDat">Kolekce vybraných dat</param>
      public void NactiVyhledaneZaznamy(ObservableCollection<Zaznam> VybranaKolekceDat)
      {
         this.VybraneZaznamy = VybranaKolekceDat;
      }

      /// <summary>
      /// Metoda pro uložení vybraných záznamů do souboru
      /// </summary>
      private void UlozeniDoSouboru()
      {
         // Vytvoření funkce pro uložení dat do souboru s nastavením nabídky formátů pro uložení
         SaveFileDialog dialog = new SaveFileDialog
         {
            Filter = "Text files (*.txt)|*.txt|Separated files (*.csv)|*.csv|Data files (*.xml)|*.xml"
         };

         // Zobrazení dialogu pro vybrání cesty k uložneí s návratovou hodnotou určující zda bylo uložení potvrzeno
         if (dialog.ShowDialog() == true)
         {
            try
            {
               // Kontrola zda byl zadán název souboru pro uložení
               if (string.IsNullOrWhiteSpace(dialog.FileName))
                  throw new ArgumentException("Zadejte název souboru!");

               // Uložení souboru voláním funkce ve správci aplikace dle zvolené možnosti typu souboru pro uložení
               switch(dialog.FilterIndex)
               {
                  case 1:  SpravaAplikace.UlozData_TXT(dialog.FileName, VybraneZaznamy);  break;
                  case 2:  SpravaAplikace.UlozData_CSV(dialog.FileName, VybraneZaznamy);  break;
                  case 3:  SpravaAplikace.UlozData_XML(dialog.FileName, VybraneZaznamy);  break;
                  default: break;
               }

               // Zavření okna pro export dat
               Close();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
         }
      }

      /// <summary>
      /// Obsluha tlačítka pro export dat do souboru
      /// </summary>
      /// <param name="sender">Zvolené tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UlozitButton_Click(object sender, RoutedEventArgs e)
      {
         UlozeniDoSouboru();
      }

      /// <summary>
      /// Obsluha tlačítka vyhledání požadovaných záznamů určených k exportu do souboru
      /// </summary>
      /// <param name="sender">Zvolené tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VyhledatButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Otevření okna pro možnost vyhledat konkrétní záznamy
            VyhledatWindow vyhledatWindow = new VyhledatWindow(KolekceZaznamu, SpravaAplikace, HlavniOkno, this, HlavniOkno.BarvaPozadi);
            vyhledatWindow.ShowDialog();

            // Aktualizace vykreslení vybraných záznamů
            VykresliZaznamy();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

   }
}
