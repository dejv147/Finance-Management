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
using System.Collections.ObjectModel;

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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře StatistikaWindow.xaml
   /// Okenní formulář slouží pro statistické zpracování dat a zobrazení zpracovaných záznamů dle zadaných parametrů.
   /// </summary>
   public partial class StatistikaWindow : Window
   {
      /// <summary>
      /// Uchování dat pro možnost zobrazení
      /// </summary>
      public ObservableCollection<Zaznam> KolekceZaznamu { get; private set; }

      /// <summary>
      /// Instance třídy pro zpracování a vykreslení statisticky zpracovaných dat
      /// </summary>
      private Statistika statistika;



      /// <summary>
      /// Konstruktor třídy pro vytvoření okenního formuláře
      /// </summary>
      /// <param name="KolekceZaznamu">Kolekce dat ke statistickému zpracování a zobrazení</param>
      /// <param name="Pozadi">Barva pozadí</param>
      public StatistikaWindow(ObservableCollection<Zaznam> KolekceZaznamu, Brush Pozadi)
      {
         try
         {
            // Inicializace okna
            InitializeComponent();

            // Nastavení barvy pozadí
            Background = Pozadi;                                  

            // Načtení kolekce dat z parametru konstruktoru 
            this.KolekceZaznamu = KolekceZaznamu;

            // Vytvoření instance třídy pro vykreslení grafů a ovládání okna pro zobrazení statisticky zpracovaných dat
            statistika = new Statistika(KolekceZaznamu, GrafCanvas, OvladaciPrvkyCanvas, InfoCanvas);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

   }
}
