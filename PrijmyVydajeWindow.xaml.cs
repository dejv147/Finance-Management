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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře PrijmyVydajeWindow.xaml
   /// Okenní formulář slouží k zobrazení záznamů v kategorii Příjem, nebo Výdaj s možností filtrace zobrazených záznamů dle určitých parametrů.
   /// </summary>
   public partial class PrijmyVydajeWindow : Window
   {
      /// <summary>
      /// Volba zda je okno určeno pro výpis příjmů nebo výdajů.
      /// </summary>
      private byte Volba;

      /// <summary>
      /// Uchování dat pro možnost zobrazení
      /// </summary>
      public ObservableCollection<Zaznam> KolekceZaznamu { get; private set; }

      /// <summary>
      /// Kolekce vybraných dat k zobrazení.
      /// </summary>
      public ObservableCollection<Zaznam> VybraneZaznamyKZobrazeni { get; private set; }

      /// <summary>
      /// Instance hlavního okna pro možnost využití funkcí hlavního okna při práci s vybraným záznamem
      /// </summary>
      private MainWindow HlavniOkno;

      /// <summary>
      /// Intance třídy pro možnost vykreslování na plátno
      /// </summary>
      private Vykreslovani vykreslovani;

      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;

      /// <summary>
      /// Instance třídy pro validaci vstupních dat
      /// </summary>
      private Validace validator;

      /// <summary>
      /// Instance třídy pro vyhledávání konkrétních dat
      /// </summary>
      private Vyhledavani vyhledavani;

      /// <summary>
      /// Instance třídy pro převedení kolekce záznamů do grafické podoby
      /// </summary>
      private GrafickyZaznam grafickyZaznam;

      /// <summary>
      /// Spodní hranice datumu pro vyhledání konkrétních záznamů
      /// </summary>
      private DateTime Datum_Od;

      /// <summary>
      /// Horní hranice datumu pro vyhledání konkrétních záznamů
      /// </summary>
      private DateTime Datum_Do;

      /// <summary>
      /// Spodní hranice hodnoty pro vyhledání konkrétních záznamů
      /// </summary>
      private double Hodnota_Od;

      /// <summary>
      /// Horní hranice hodnoty pro vyhledání konkrétních záznamů
      /// </summary>
      private double Hodnota_Do;

      /// <summary>
      /// Pomocná proměnná pro výpis příjmů nebo výdajů
      /// </summary>
      private string slovo;



      /// <summary>
      /// Konstruktor třídy pro inicializaci okna a úvodního nastavení.
      /// </summary>
      /// <param name="PrijemNeboVydaj">0 - Okno pro Výdaje, 1 - Okno pro příjmy</param>
      /// <param name="KolekceZaznamu">Kolekce záznamů pro zobrazení a zpracování</param>
      /// <param name="SpravaAplikace">Instance správce aplikace</param>
      /// <param name="HlavniOkno">Instance hlavního okna</param>
      /// <param name="Pozadi">Barva pozadí</param>
      public PrijmyVydajeWindow(byte PrijemNeboVydaj, ObservableCollection<Zaznam> KolekceZaznamu, Spravce_Aplikace SpravaAplikace, MainWindow HlavniOkno, Brush Pozadi)
      {
         InitializeComponent();

         // Nastavení barvy pozadí
         Background = Pozadi;

         // Určení volby zobrazení okna
         Volba = PrijemNeboVydaj;

         // Načtení kolekce dat z parametru konstruktoru
         this.KolekceZaznamu = KolekceZaznamu;

         // Převzetí instance správce do interní proměnné
         this.SpravaAplikace = SpravaAplikace;

         // Předání instance hlavního okna
         this.HlavniOkno = HlavniOkno;

         // Úvodní inicializace tříd a pomocných proměnných
         vykreslovani = new Vykreslovani();
         vyhledavani = new Vyhledavani();
         validator = new Validace();
         grafickyZaznam = new GrafickyZaznam(HlavniOkno, InfoBublinaCanvas);
         VybraneZaznamyKZobrazeni = KolekceZaznamu;

         // Úvodní nastavení okna dle volby zobrazení
         if (Volba == 1)
            NastavOknoNaPrijem();
         else
            NastavOknoNaVydaj();

         // Úvodní nastavení zadávacích polí
         DatumMIN_DatePicker.SelectedDate = DateTime.Now;
         DatumMAX_DatePicker.SelectedDate = DateTime.Now;


         // Úvodní vykreslení seznamu záznamů
         VykresliSeznamZazanamu();
      }



      /// <summary>
      /// Pomocná metoda pro nastavení proměnných parametrů dle zvoleného módu okna.
      /// </summary>
      private void NastavOknoNaPrijem()
      {
         Title = "Příjmy";
         slovo = "příjmů";
      }

      /// <summary>
      /// Pomocná metoda pro nastavení proměnných parametrů dle zvoleného módu okna.
      /// </summary>
      private void NastavOknoNaVydaj()
      {
         Title = "Výdaje";
         slovo = "výdajů";
      }

      /// <summary>
      /// Výpočet celkové hodnoty a počtu vybraných záznamů. 
      /// Získané hodnoty jsou vypsány do zobrazovacího okna.
      /// </summary>
      private void VypisSouhrn()
      {
          // Počet vybraných záznamů
          int pocet = VybraneZaznamyKZobrazeni.Count;

          // Výpočet celkové hodnoty vybraných záznamů
          double hodnota = 0;
          foreach(Zaznam zaznam in VybraneZaznamyKZobrazeni)
          {
              hodnota += zaznam.Hodnota_PrijemVydaj;
          }

          // Výpis získaných údajů do okna
          CelkovaHodnotaTextBlock.Text = "Celková výše " + slovo + " je " + hodnota.ToString() + " Kč";
          PocetZaznamuTextBlock.Text = "Bylo nalezeno " + pocet.ToString() + " záznamů";
      }

      /// <summary>
      /// Metoda pro vykreslení seznamu záznamů včetně souhrného přehledu
      /// </summary>
      private void VykresliSeznamZazanamu()
      {
         // Smazání obsahu plátna pro vykreslení záznamů
         SeznamZaznamuCANVAS.Children.Clear();

         // Zrušení označení vybraného záznamu
         HlavniOkno.ZrusOznaceniZaznamu();

         // Načtení kolekce dat ke grafickému zpracování
         grafickyZaznam.ObnovKolekciZaznamu(VybraneZaznamyKZobrazeni);

         // Vykreslení zpracovaného seznamu záznamů do zobrazeného okna
         vykreslovani.VykresliPrvek(SeznamZaznamuCANVAS, grafickyZaznam.StrankaZaznamu);

        // Vypsání souhrného přehledu
        VypisSouhrn();
      }

      /// <summary>
      /// Metoda pro vyhledání záznamů dle zadaných kritérií.
      /// </summary>
      private void ZobrazVyhledaneZaznamy()
      {
         // Pokud není vybráno kritérium pro hledání zobrazí se chybová zpráva
         if (HledatDleHodnoty_CheckBox.IsChecked == false && HledatDleDatumu_CheckBox.IsChecked == false)
            throw new ArgumentException("Zvolte kritérium pro hledání");

         // Je zatrženo vyhledávání dle data
         if (HledatDleDatumu_CheckBox.IsChecked == true)
         {
                // Načtení vyhledaných dat do interní kolekce pro vykreslení
                VybraneZaznamyKZobrazeni = vyhledavani.VratZaznamyVCasovemRozmezi(VybraneZaznamyKZobrazeni, Datum_Od, Datum_Do);
         }

         // Je zatrženo vyhledávání dle hodnoty
         if (HledatDleHodnoty_CheckBox.IsChecked == true)
         {
            if (!(Hodnota_Od.ToString().Length > 0))
               throw new ArgumentException("Zadejte minimální hodnotu pro vyhledávání");

            if (!(Hodnota_Do.ToString().Length > 0))
               throw new ArgumentException("Zadejte maximální hodnotu pro vyhledávání");

            // Načtení vyhledaných dat do interní kolekce pro vykreslení
            VybraneZaznamyKZobrazeni = vyhledavani.VratZaznamySHodnotouVRozmezi(VybraneZaznamyKZobrazeni, Hodnota_Od, Hodnota_Do);
         }

         // Upozornění pro případ že zadaným kritértiím nevyhovují žádné záznamy
         if (!(VybraneZaznamyKZobrazeni.Count > 0))
         {
                VybraneZaznamyKZobrazeni = KolekceZaznamu;
                throw new ArgumentException("Nebyly nalezeny žádné záznamy");
         }
             

         // Vykreslení aktualizované kolekce dat pro zobrazení
         VykresliSeznamZazanamu();
      }


      /// <summary>
      /// Obsluha tlačítka pro vyhledání záznamů dle zadaných parametrů
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VyhledatButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            ZobrazVyhledaneZaznamy();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      /// <summary>
      /// Obsluha tlačítka pro zrušení všech filtrů vyhledávání. 
      /// Metoda nahraje do kolekce dat pro zobrazení kolekci všech dat a obnoví vykreslený seznam záznamů.
      /// </summary>
      /// <param name="sender">Tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZrusitButton_Click(object sender, RoutedEventArgs e)
      {
         VybraneZaznamyKZobrazeni = KolekceZaznamu;
         VykresliSeznamZazanamu();
      }


      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HodnotaMIN_TextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
            try
            {
                if (HodnotaMIN_TextBox.Text.Length > 0)
                    Hodnota_Od = validator.NactiCislo(HodnotaMIN_TextBox.Text);
                else
                    Hodnota_Od = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
                HodnotaMIN_TextBox.Text = Hodnota_Od.ToString();
            }
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HodnotaMAX_TextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
            try
            {
                if (HodnotaMAX_TextBox.Text.Length > 0)
                    Hodnota_Do = validator.NactiCislo(HodnotaMAX_TextBox.Text);
                else
                    Hodnota_Do = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
                HodnotaMAX_TextBox.Text = Hodnota_Do.ToString();
            }
        }

      /// <summary>
      /// Načtení data do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumMIN_DatePicker_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
            try
            {
                Datum_Od = validator.NactiDatum(DatumMIN_DatePicker.SelectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

      /// <summary>
      /// Načtení data do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumMIN_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
            try
            {
                Datum_Od = validator.NactiDatum(DatumMIN_DatePicker.SelectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
      }

      /// <summary>
      /// Načtení data do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumMAX_DatePicker_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
            try
            {
                Datum_Do = validator.NactiDatum(DatumMAX_DatePicker.SelectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

      /// <summary>
      /// Načtení data do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumMAX_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
            try
            {
                Datum_Do = validator.NactiDatum(DatumMAX_DatePicker.SelectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

   }
}
