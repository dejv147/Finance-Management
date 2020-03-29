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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře UpravitPolozkuWindow.xaml
   /// Okenní formulář slouží pro úpravu konkrétní položky (možnost změnit parametry dané položky).
   /// </summary>
   public partial class UpravitPolozkuWindow : Window
   {
      /// <summary>
      /// Instance okna pro přidávání položek
      /// </summary>
      private PridatPolozkyWindow OknoPolozek;

      /// <summary>
      /// Položka, jejíž data jsou upravována
      /// </summary>
      private Polozka UpravovanaPolozka;

      /// <summary>
      /// Instance třídy pro validaci vstupních dat
      /// </summary>
      private Validace validator;

      /// <summary>
      /// Název položky
      /// </summary>
      private string Nazev;

      /// <summary>
      /// Hodnota položky
      /// </summary>
      private double Cena;

      /// <summary>
      /// Stručný popis položky
      /// </summary>
      private string Popis;

      /// <summary>
      /// Kategorie do které spadá konkrétní položka
      /// </summary>
      private Kategorie KategoriePolozky;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;



      /// <summary>
      /// Konstruktor třídy pro inicializaci okna a úvodní nastavení potřebných parametrů.
      /// </summary>
      /// <param name="OknoPolozky">Instance okna pro přidání položky</param>
      /// <param name="polozka">Položka určena k úpravě</param>
      public UpravitPolozkuWindow(PridatPolozkyWindow OknoPolozky, Polozka polozka)
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Inicializace interních proměných
         OknoPolozek = OknoPolozky;
         UpravovanaPolozka = polozka;
         validator = new Validace();

         // Načtení parametrů předané položky
         Nazev = UpravovanaPolozka.Nazev;
         Cena = UpravovanaPolozka.Cena;
         Popis = UpravovanaPolozka.Popis;
         KategoriePolozky = UpravovanaPolozka.KategoriePolozky;
         ZavrenoBezUlozeni = 1;

         // Úvodní vypsání načtených parametrů do upravovacího okna
         NazevPolozkyTextBox.Text = Nazev;
         CenaTextBox.Text = Cena.ToString();
         PopisTextBox.Text = Popis;
         KategorieComboBox.SelectedIndex = (int)KategoriePolozky;
      }



      /// <summary>
      /// Uložení zadaného názvu do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void NazevPolozkyTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Nazev = NazevPolozkyTextBox.Text.ToString();
      }

      /// <summary>
      /// Uložení zadaného čísla do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void CenaTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         try
         {
            if (CenaTextBox.Text.Length > 0)
               Cena = validator.NactiCislo(CenaTextBox.Text);
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Zobrazení do boxu načtené hodnoty (smazání neplatného obsahu)
            CenaTextBox.Text = Cena.ToString();
         }
      }

      /// <summary>
      /// Uložení konkrétního výčtového typu dle indexu zvoleného typu rozbalovací nabídky
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void KategorieComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         KategoriePolozky = (Kategorie)KategorieComboBox.SelectedIndex;
      }

      /// <summary>
      /// Uložení textového popisu do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PopisTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Popis = PopisTextBox.Text.ToString();
      }

      /// <summary>
      /// Uložení provedených změn a zavření okna
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UlozitButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Kontrola zadaných dat
            KontrolaZadanychDat();

            // Uložení provedených změn
            UpravovanaPolozka.Nazev = Nazev;
            UpravovanaPolozka.Cena = Cena;
            UpravovanaPolozka.KategoriePolozky = KategoriePolozky;
            UpravovanaPolozka.Popis = Popis;

            // Nastavení příznakového bitu informující, že byly provedeny úpravy dat
            OknoPolozek.PolozkaUpravena = 1;

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
      /// Pomocná metoda pro kontrolu zadaných údajů při vytváření nové položky.
      /// </summary>
      private void KontrolaZadanychDat()
      {
         // Kontrola zda byl zadán název
         if (!(Nazev.Length > 0))
            throw new ArgumentException("Zadejte název!");

         // Kontrola zda byla zadána cena
         if (!(Cena.ToString().Length > 0))
            throw new ArgumentException("Zadejte cenu!");

         // Kontrola zda byla vybrána kategorie
         if (KategoriePolozky == Kategorie.Nevybrano)
            throw new ArgumentException("Vyberte kategorii!");
      }

      /// <summary>
      /// Zobrazení upozornění při zavírání okna.
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
