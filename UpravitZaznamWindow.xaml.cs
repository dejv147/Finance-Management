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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře UpravitZaznamWindow.xaml
   /// Okenní formulář slouží pro úpravu konkrétního záznamu (možnost změnit parametry daného záznamu).
   /// </summary>
   public partial class UpravitZaznamWindow : Window
   {
      /// <summary>
      /// Záznam k upravení
      /// </summary>
      private Zaznam UpravovanyZaznam;

      /// <summary>
      /// Instance třídy pro grafické zobrazení položek
      /// </summary>
      private GrafickePolozky grafickePolozky;

      /// <summary>
      /// Intance třídy pro možnost vykreslování na plátno
      /// </summary>
      private Vykreslovani vykreslovani;

      /// <summary>
      /// Instance třídy pro validaci vstupních dat
      /// </summary>
      private Validace validator;

      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;

      /// <summary>
      /// Název záznamu
      /// </summary>
      private string Nazev;

      /// <summary>
      /// Datum záznamu
      /// </summary>
      private DateTime Datum;

      /// <summary>
      /// Hodnota celkového příjmu nebo výdaje daného záznamu
      /// </summary>
      private double PrijemVydaj_Hodnota;

      /// <summary>
      /// Poznámka k záznamu
      /// </summary>
      private string Poznamka;

      /// <summary>
      /// Vybraná kategorie záznamu
      /// </summary>
      private Kategorie KategorieZaznamu;

      /// <summary>
      /// Značka zda se jedná o výdaj nebo příjem
      /// </summary>
      private KategoriePrijemVydaj PrijemNeboVydaj;

      /// <summary>
      /// Seznam položek patřících danému záznamu
      /// </summary>
      private ObservableCollection<Polozka> SeznamPolozek;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;


      /// <summary>
      /// Konstruktor třídy
      /// </summary>
      /// <param name="zaznam">Záznam určený k úpravě</param>
      /// <param name="Spravce">Instance správce aplikace</param>
      public UpravitZaznamWindow(Zaznam zaznam, Spravce_Aplikace Spravce)
      {
         // Inicializace interních proměnných
         InitializeComponent();                                
         SpravaAplikace = Spravce;                         
         validator = new Validace();                        
         ZavrenoBezUlozeni = 1;                              
         UpravovanyZaznam = zaznam;                           
         grafickePolozky = new GrafickePolozky(SeznamPolozek, this);
         vykreslovani = new Vykreslovani();

         // Načtení parametrů předaného záznamu
         Nazev = zaznam.Nazev;
         Datum = zaznam.Datum;
         PrijemVydaj_Hodnota = zaznam.Hodnota_PrijemVydaj;
         Poznamka = zaznam.Poznamka;
         KategorieZaznamu = zaznam.Kategorie;
         PrijemNeboVydaj = zaznam.PrijemNeboVydaj;
         SeznamPolozek = zaznam.SeznamPolozek;

         // Úvodní vypsání načtených parametrů do upravovacího okna
         NazevZaznamuTextBox.Text = Nazev;
         DatumZaznamuDatePicker.SelectedDate = Datum;
         PrijemVydajTextBox.Text = PrijemVydaj_Hodnota.ToString();
         PrijemVydajComboBox.SelectedIndex = (int)PrijemNeboVydaj;
         KategorieComboBox.SelectedIndex = (int)KategorieZaznamu;

         // Úvodní vykreslení seznamu položek
         VykresliSeznamPolozek();
      }



      /// <summary>
      /// Metoda pro vykreslení seznamu položek v grafické formě na plátno v okenním formuláři.
      /// </summary>
      private void VykresliSeznamPolozek()
      {
         // Smazání obsahu pro opětovné vykreslení nových dat
         SeznamPolozekCanvas.Children.Clear();
         
         // Pokud seznam neobsahuje žádnou položku, nevykreslí se
         if (!(SeznamPolozek.Count > 0))
            return;
         
         // Aktualizace dat pro grafické zpracování
         grafickePolozky.ObnovKolekciPolozek(SeznamPolozek);

         // Vykreslení stránky položek na plátno
         vykreslovani.VykresliPrvek(SeznamPolozekCanvas, grafickePolozky.StrankaPolozek);
      }

      /// <summary>
      /// Pomocná metoda pro kontrolu zadaných údajů při vytváření nového záznamu
      /// </summary>
      private void KontrolaZadanychDat()
      {
         // Kontrola zda byl zadán název
         if (!(Nazev.Length > 0))
            throw new ArgumentException("Zadejte název!");

         // Kontrola zda byla zadána hodnota příjmu/výdaje
         if (!(PrijemVydajComboBox.SelectedIndex == 0 || PrijemVydajComboBox.SelectedIndex == 1))
            throw new ArgumentException("Zvolte zda se jedná o příjem nebo výdaj");

         // Kontrola zda byla zadána hodnota příjmu/výdaje
         if (!(PrijemVydaj_Hodnota.ToString().Length > 0))
            throw new ArgumentException("Zadejte hodnotu");

         // Kontrola zda byla vybrána kategorie
         if (KategorieZaznamu == Kategorie.Nevybrano)
            throw new ArgumentException("Vyberte kategorii!");
      }


      /// <summary>
      /// Obslužná metoda pro uložení provedených úprav
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UlozitButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Kontrola zadaných dat od uživatele
            KontrolaZadanychDat();

            // Uložení upravených dat do upravovaného záznamu
            UpravovanyZaznam.Nazev = Nazev;
            UpravovanyZaznam.Datum = Datum;
            UpravovanyZaznam.Hodnota_PrijemVydaj = PrijemVydaj_Hodnota;
            UpravovanyZaznam.PrijemNeboVydaj = PrijemNeboVydaj;
            UpravovanyZaznam.Poznamka = Poznamka;
            UpravovanyZaznam.Kategorie = KategorieZaznamu;
            UpravovanyZaznam.SeznamPolozek = SeznamPolozek;

            // Zavření okna
            ZavrenoBezUlozeni = 0;
            Close();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Uložení zadaného názvu do interní proměnné.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void NazevZaznamuTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Nazev = NazevZaznamuTextBox.Text.ToString();
      }

      /// <summary>
      /// Načtení vybraného data do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumZaznamuDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            Datum = validator.NactiDatum(DatumZaznamuDatePicker.SelectedDate);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      /// <summary>
      /// Uložení značky zda se vytváří příjem nebo výdaj.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrijemVydajComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         PrijemNeboVydaj = PrijemVydajComboBox.SelectedIndex == 0 ? KategoriePrijemVydaj.Prijem : KategoriePrijemVydaj.Vydaj;
      }

      /// <summary>
      /// Obsluha udáosti vyvolané při zadávání hodnoty do textového bloku. 
      /// Při zadávání se zadané číslo načítá do interní proměnné pro následné zpracování.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrijemVydajTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (PrijemVydajTextBox.Text.Length > 0)
               PrijemVydaj_Hodnota = validator.NactiCislo(PrijemVydajTextBox.Text);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení do boxu načtenou hodnotu (smazání neplatného obsahu)
            PrijemVydajTextBox.Text = PrijemVydaj_Hodnota.ToString();
         }
      }

      /// <summary>
      /// Uloží konkrétní výčtový typ do kategorie dle zvoleného typu z rozbalovací nabídky.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void KategorieComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         KategorieZaznamu = (Kategorie)KategorieComboBox.SelectedIndex;
      }

      /// <summary>
      /// Otevření okenního formuláře pro úpravu poznámky k záznamu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UpravitPoznamku_Click(object sender, RoutedEventArgs e)
      {
         PridatPoznamkuWindow pridatPoznamkuWindow = new PridatPoznamkuWindow(this, Poznamka);
         pridatPoznamkuWindow.ShowDialog();
      }

      /// <summary>
      /// Otevření okna pro úpravu položek
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UpravitPolozky_Click(object sender, RoutedEventArgs e)
      {
         PridatPolozkyWindow pridatPolozkyWindow = new PridatPolozkyWindow(this, SeznamPolozek);
         pridatPolozkyWindow.ShowDialog();

         // Aktualizace vykresleného seznamu
         VykresliSeznamPolozek();
      }


      /// <summary>
      /// Metoda volaná z okenního formuláře pro upravení poznámky k záznamu.
      /// </summary>
      /// <param name="TextPoznamky">Předaný textový řetězec poznámky</param>
      public void NastavPoznamku(string TextPoznamky)
      {
         this.Poznamka = TextPoznamky;
      }

      /// <summary>
      /// Metoda volána z okna pro přidání položek. 
      /// Přidá seznam položek k aktuálnímu záznamu.
      /// </summary>
      /// <param name="SeznamPolozek">Vytvořený seznam položek</param>
      public void UlozUpravenePolozky(ObservableCollection<Polozka> SeznamPolozek)
      {
         this.SeznamPolozek = SeznamPolozek;
      }


      /// <summary>
      /// Zobrazení upozornění při zavírání okna
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         // Pokud je okno zavřeno křížkem (bez uložení dat) zobrazí se varovné okno
         if (ZavrenoBezUlozeni == 1)
         {
            MessageBoxResult VybranaMoznost = MessageBox.Show("Provedené změny nebudou uloženy! \nChcete pokračovat?", "Pozor", MessageBoxButton.YesNo, MessageBoxImage.Question);

            switch (VybranaMoznost)
            {
               case MessageBoxResult.Yes:
                  break;

               case MessageBoxResult.No:
                  e.Cancel = true;
                  break;
            }
         }

         // Pokud je okno zavřeno voláním funkce (s uložením dat) tak se okno zavře bez varování
         else
         {
            e.Cancel = false;
         }
      }

   }
}
