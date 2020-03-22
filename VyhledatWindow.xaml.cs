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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře VyhledatWindow.xaml
   /// Okenní formulář slouží pro vyhledávání záznamů dle zadaných parametrů filtrů.
   /// </summary>
   public partial class VyhledatWindow : Window
   {
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
      /// Instance okna pro export dat. Slouží pro možnost navrácení kolekce vybraných záznamů v tomto okně.
      /// </summary>
      private ExportDatWindow ExportDat;

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
      /// Název záznamu
      /// </summary>
      private string Nazev;

      /// <summary>
      /// Pomocná proměnná pro uchování data začátku aktuálního měsíce
      /// </summary>
      private DateTime PrvniDenAktualnihoMesice;

      /// <summary>
      /// Spodní hranice datumu pro vyhledání konkrétních záznamů
      /// </summary>
      private DateTime Datum_MIN;

      /// <summary>
      /// Horní hranice datumu pro vyhledání konkrétních záznamů
      /// </summary>
      private DateTime Datum_MAX;

      /// <summary>
      /// Spodní hranice hodnoty pro vyhledání konkrétních záznamů
      /// </summary>
      private double Hodnota_MIN;

      /// <summary>
      /// Horní hranice hodnoty pro vyhledání konkrétních záznamů
      /// </summary>
      private double Hodnota_MAX;

      /// <summary>
      /// Kategorie záznamu
      /// </summary>
      private Kategorie kategorie;

      /// <summary>
      /// Spodní hranice počtu položek pro vyhledání konkrétních záznamů
      /// </summary>
      private int PocetPolozek_MIN;

      /// <summary>
      /// Horní hranice počtu položek pro vyhledání konkrétních záznamů
      /// </summary>
      private int PocetPolozek_MAX;



      /// <summary>
      /// Konstruktor třídy pro úvodní nastavení potřebných atributů a interních proměnných při inicializaci okna.
      /// </summary>
      /// <param name="KolekceZaznamu">Kolekce záznamů pro vykreslení a zpracování</param>
      /// <param name="SpravaAplikace">Instance správce aplikace</param>
      /// <param name="HlavniOkno">Instance hlavního okna</param>
      /// <param name="Pozadi">Barva pozadí</param>
      public VyhledatWindow(ObservableCollection<Zaznam> KolekceZaznamu, Spravce_Aplikace SpravaAplikace, MainWindow HlavniOkno,ExportDatWindow exportDatWindow, Brush Pozadi)
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Nastavení barvy pozadí
         Background = Pozadi;

         // Načtení kolekce dat z parametru konstruktoru 
         this.KolekceZaznamu = KolekceZaznamu;

         // Převzetí instance správce do interní proměnné
         this.SpravaAplikace = SpravaAplikace;

         // Předání instance hlavního okna
         this.HlavniOkno = HlavniOkno;

         // Nastavení interní instance okna pro export dat podle toho, zda je okno otevřeno z hlavního okna nebo z okna pro export dat
         if (exportDatWindow != null)
            this.ExportDat = exportDatWindow;
         else
            this.ExportDat = null;

         // Úvodní inicializace tříd a pomocných proměnných
         vykreslovani = new Vykreslovani();
         vyhledavani = new Vyhledavani();
         validator = new Validace();
         grafickyZaznam = new GrafickyZaznam(HlavniOkno, InfoBublinaCanvas);
         VybraneZaznamyKZobrazeni = KolekceZaznamu;
         Nazev = "";
         kategorie = Kategorie.Nevybrano;

         // Nastavení prvního dne aktuálního měsíce
         PrvniDenAktualnihoMesice = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

         // Nastavení data do pole pro zadávání
         DatumMIN_DatePicker.SelectedDate = PrvniDenAktualnihoMesice;
         DatumMAX_DatePicker.SelectedDate = DateTime.Now.Date;

         // Úvodní vykreslení seznamu záznamů
         VykresliSeznamZazanamu();
      }




      /// <summary>
      /// Metoda pro vykreslení seznamu záznamů
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
      /// Výpočet celkové hodnoty a počtu vybraných záznamů. 
      /// Získané hodnoty jsou vypsány do zobrazovacího okna.
      /// </summary>
      private void VypisSouhrn()
      {
         // Počet vybraných záznamů
         int pocet = VybraneZaznamyKZobrazeni.Count;

         // Výpočet celkové hodnoty vybraných záznamů
         double hodnota = 0;
         foreach (Zaznam zaznam in VybraneZaznamyKZobrazeni)
         {
            hodnota += zaznam.Hodnota_PrijemVydaj;
         }

         // Výpis získaných údajů do okna
         PocetZaznamuTextBlock.Text = " Bylo nalezeno " + pocet.ToString() + " záznamů";
         CelkovaHodnotaTextBlock.Text = " Jejich celková hodnota je " + hodnota.ToString() + " Kč  ";
      }

      /// <summary>
      /// Vyhledání záznamů s určitým názvem
      /// </summary>
      private void HledejNazev()
      {
         // Načtení záznamů s požadovaným názvem
         VybraneZaznamyKZobrazeni = vyhledavani.VratZaznamyDleNazvu(VybraneZaznamyKZobrazeni, Nazev);

         // Pokud není nalezen žádný odpovídající záznam obnoví se vybraná kolekce dat načtením celkové kolekce dat
         if (VybraneZaznamyKZobrazeni.Count == 0)
         {
            VybraneZaznamyKZobrazeni = KolekceZaznamu;
            throw new ArgumentException("Nebyly nalezeny žádné záznamy se zadaným názvem!");
         }
      }

      /// <summary>
      /// Vyhledání záznamů v určitém časovém období
      /// </summary>
      private void HledejDatum()
      {
         // Načtení záznamů v zadaném časovém období
         VybraneZaznamyKZobrazeni = vyhledavani.VratZaznamyVCasovemRozmezi(VybraneZaznamyKZobrazeni, Datum_MIN, Datum_MAX);

         // Pokud není nalezen žádný odpovídající záznam obnoví se vybraná kolekce dat načtením celkové kolekce dat
         if (VybraneZaznamyKZobrazeni.Count == 0)
         {
            VybraneZaznamyKZobrazeni = KolekceZaznamu;
            throw new ArgumentException("Nebyly nalezeny žádné záznamy v zadaném období!");
         }
      }

      /// <summary>
      /// Vyhledání záznamů s určitou hodnotou
      /// </summary>
      private void HledejHodnotu()
      {
         // Načtení záznamů s hodnotou v zadaném rozmezí
         VybraneZaznamyKZobrazeni = vyhledavani.VratZaznamySHodnotouVRozmezi(VybraneZaznamyKZobrazeni, Hodnota_MIN, Hodnota_MAX);

         // Pokud není nalezen žádný odpovídající záznam obnoví se vybraná kolekce dat načtením celkové kolekce dat
         if (VybraneZaznamyKZobrazeni.Count == 0)
         {
            VybraneZaznamyKZobrazeni = KolekceZaznamu;
            throw new ArgumentException("Nebyly nalezeny žádné záznamy s danou hodnotou!");
         }
      }

      /// <summary>
      /// Vyhledání záznamů v určité kategorii
      /// </summary>
      private void HledejKategorii()
      {
         // Načtení záznamů v požadované kategorii
         VybraneZaznamyKZobrazeni = vyhledavani.VratZaznamyDleKategorie(VybraneZaznamyKZobrazeni, kategorie);

         // Pokud není nalezen žádný odpovídající záznam obnoví se vybraná kolekce dat načtením celkové kolekce dat
         if (VybraneZaznamyKZobrazeni.Count == 0)
         {
            VybraneZaznamyKZobrazeni = KolekceZaznamu;
            throw new ArgumentException("Nebyly nalezeny žádné záznamy v požadované kategorii!");
         }
      }

      /// <summary>
      /// Vyhledání všech příjmů
      /// </summary>
      private void HledejPrijem()
      {
         // Načtení všech příjmů
         VybraneZaznamyKZobrazeni = vyhledavani.VratPrijmy(VybraneZaznamyKZobrazeni);

         // Pokud není nalezen žádný odpovídající záznam obnoví se vybraná kolekce dat načtením celkové kolekce dat
         if (VybraneZaznamyKZobrazeni.Count == 0)
         {
            VybraneZaznamyKZobrazeni = KolekceZaznamu;
            throw new ArgumentException("Nebyly nalezeny žádné příjmy!");
         }
      }

      /// <summary>
      /// Vyhledání všech výdajů
      /// </summary>
      private void HledejVydaj()
      {
         // Načtení všech výdajů
         VybraneZaznamyKZobrazeni = vyhledavani.VratVydaje(VybraneZaznamyKZobrazeni);

         // Pokud není nalezen žádný odpovídající záznam obnoví se vybraná kolekce dat načtením celkové kolekce dat
         if (VybraneZaznamyKZobrazeni.Count == 0)
         {
            VybraneZaznamyKZobrazeni = KolekceZaznamu;
            throw new ArgumentException("Nebyly nalezeny žádné výdaje!");
         }
      }

      /// <summary>
      /// Vyhledání záznamů s určitým počtem položek
      /// </summary>
      private void HledejPolozky()
      {
         // Načtení záznamů s požadovaným počtem položek
         VybraneZaznamyKZobrazeni = vyhledavani.VratZaznamyDlePoctuPolozek(VybraneZaznamyKZobrazeni, PocetPolozek_MIN, PocetPolozek_MAX);

         // Pokud není nalezen žádný odpovídající záznam obnoví se vybraná kolekce dat načtením celkové kolekce dat
         if (VybraneZaznamyKZobrazeni.Count == 0)
         {
            VybraneZaznamyKZobrazeni = KolekceZaznamu;
            throw new ArgumentException("Nebyly nalezeny žádné záznamy s požadovaným počtem položek!");
         }
      }




      /// <summary>
      /// Obluha tlačítka pro vyhledání záznamů dle zadaných parametrů.
      /// Kontrola zvolených filtrů a následné vyhledání záznamů dle zadaných kritérií.
      /// </summary>
      /// <param name="sender">Tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VyhledatButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Pokud je zvoleno zobrazení příjmů a zároveň není zvoleno zobrazení výdajů, vyhledají se všechny příjmy
            if (Prijmy_CheckBox.IsChecked == true && Vydaje_CheckBox.IsChecked == false)
               HledejPrijem();

            // Pokud je zvoleno zobrazení výdajů a zároveň není zvoleno zobrazení příjmů, vyhledají se všechny výdaje
            if (Vydaje_CheckBox.IsChecked == true & Prijmy_CheckBox.IsChecked == false)
               HledejVydaj();

            // Kontrola zda je zvoleno zobrazení příjmů nebo výdajů
            if (Prijmy_CheckBox.IsChecked == false && Vydaje_CheckBox.IsChecked == false)
               throw new ArgumentException("Zvolte jaké záznamy chcete vyhledat! \n\tPříjmy - Výdaje");


            // Vyhledání záznamů se zadaným názvem
            if (Nazev_CheckBox.IsChecked == true)
            {
               // Kontrola zda byl zadán název
               if (!(Nazev.Length > 0))
                  throw new ArgumentException("Zadejte název!");

               // Vyhledání záznamů se zadaným názvem
               HledejNazev();
            }

            // Vyhledání záznamů dle data
            if (Datum_CheckBox.IsChecked == true)
               HledejDatum();
           
            // Vyhledání záznamů dle hodnoty
            if (Hodnota_CheckBox.IsChecked == true)
            {
               if (!(Hodnota_MIN.ToString().Length > 0))
                  throw new ArgumentException("Zadejte minimální hodnotu pro vyhledávání");

               if (!(Hodnota_MAX.ToString().Length > 0))
                  throw new ArgumentException("Zadejte maximální hodnotu pro vyhledávání");

               // Vyhledání záznamů v zadaném rozmezí hodnoty
               HledejHodnotu();
            }

            // Vyhledání záznamů dle kategorie
            if (Kategorie_CheckBox.IsChecked == true)
            {
               // Kontrola zda byla vybrána kategorie
               if (kategorie == Kategorie.Nevybrano)
                  throw new ArgumentException("Vyberte kategorii!");

               // Vyhledání záznamu dle kategorie
               HledejKategorii();
            }

            // Vyhledání dle počtu položek
            if (Polozky_CheckBox.IsChecked == true)
            {
               if (!(PocetPolozek_MIN.ToString().Length > 0))
                  throw new ArgumentException("Zadejte minimální počet položek pro vyhledávání");

               if (!(PocetPolozek_MAX.ToString().Length > 0))
                  throw new ArgumentException("Zadejte maximální počet položek pro vyhledávání");

               // Vyhledání záznamu s počtem položek v zadaném rozmezí
               HledejPolozky();
            }


            // Upozornění pro případ že zadaným kritértiím nevyhovují žádné záznamy
            if (!(VybraneZaznamyKZobrazeni.Count > 0))
            {
               VybraneZaznamyKZobrazeni = KolekceZaznamu;
               throw new ArgumentException("Nebyly nalezeny žádné záznamy");
            }

            // Vykreslení aktualizovaného seznamu záznamů s vyhledanými záznamy
            VykresliSeznamZazanamu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         } 
      }

      /// <summary>
      /// Obsluha tlačítka pro odebrání záznamu.
      /// Předá vyvolanou událost do hlavního okna, kde je tlačítko se stejnou funkcí.
      /// </summary>
      /// <param name="sender">Tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void OdebratButton_Click(object sender, RoutedEventArgs e)
      {
         HlavniOkno.OdebratZaznam_Click(sender, e);
      }

      /// <summary>
      /// Obsluha tlačítka pro zrušení všech filtrů vyhledávání. 
      /// Metoda nahraje do kolekce dat pro zobrazení kolekci všech dat a obnoví vykreslený seznam záznamů.
      /// </summary>
      /// <param name="sender">Tlačítko</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZrusitButton_Click(object sender, RoutedEventArgs e)
      {
         // Zrušení označení filtrů pro vyhledávání
         Nazev_CheckBox.IsChecked = false;
         Datum_CheckBox.IsChecked = false;
         Hodnota_CheckBox.IsChecked = false;
         Kategorie_CheckBox.IsChecked = false;
         Polozky_CheckBox.IsChecked = false;
         Prijmy_CheckBox.IsChecked = true;
         Vydaje_CheckBox.IsChecked = true;

         // Vykreslení všech záznamů
         VybraneZaznamyKZobrazeni = KolekceZaznamu;
         VykresliSeznamZazanamu();
      }


      /// <summary>
      /// Načtení zadaného textu do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void NazevTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Nazev = NazevTextBox.Text.ToString();
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HodnotaMAXTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (HodnotaMAXTextBox.Text.Length > 0)
               Hodnota_MAX = validator.NactiCislo(HodnotaMAXTextBox.Text);
            else
               Hodnota_MAX = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            HodnotaMAXTextBox.Text = Hodnota_MAX.ToString();
         }
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void HodnotaMINTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (HodnotaMINTextBox.Text.Length > 0)
               Hodnota_MIN = validator.NactiCislo(HodnotaMINTextBox.Text);
            else
               Hodnota_MIN = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            HodnotaMINTextBox.Text = Hodnota_MIN.ToString();
         }
      }

      /// <summary>
      /// Načtení vybraného data do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumMIN_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            Datum_MIN = validator.NactiDatum(DatumMIN_DatePicker.SelectedDate);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Načtení vybraného data do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumMAX_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            Datum_MAX = validator.NactiDatum(DatumMAX_DatePicker.SelectedDate);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Uložení konkrétního výčtového typu do kategorie dle zvoleného typu z rozbalovací nabídky.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void KategorieComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         kategorie = (Kategorie)KategorieComboBox.SelectedIndex;
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PolozkyMINTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (PolozkyMINTextBox.Text.Length > 0)
               PocetPolozek_MIN = (int)validator.NactiCislo(PolozkyMINTextBox.Text);
            else
               PocetPolozek_MIN = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            PolozkyMINTextBox.Text = PocetPolozek_MIN.ToString();
         }
      }

      /// <summary>
      /// Načtení číselné hodnoty do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PolozkyMAXTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (PolozkyMAXTextBox.Text.Length > 0)
               PocetPolozek_MAX = (int)validator.NactiCislo(PolozkyMAXTextBox.Text);
            else
               PocetPolozek_MAX = 0;
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení hodnoty z interní proměnné (smazání neplatného obsahu)
            PolozkyMAXTextBox.Text = PocetPolozek_MAX.ToString();
         }
      }

      /// <summary>
      /// Obsluha zavírání okna
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         // Pokud bylo okno Vyhledavani otevřeno z okna proexport dat, při zavírání se do tohoto okna pošlou vyhledané záznamy
         if (ExportDat != null)
            ExportDat.NactiVyhledaneZaznamy(VybraneZaznamyKZobrazeni);
      }
   }
}
