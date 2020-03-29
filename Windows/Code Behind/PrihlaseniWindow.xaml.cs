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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře PrihlaseniWindow.xaml
   /// Třída obsluhuje okenní formulář pro přihlášení uživatele.
   /// </summary>
   public partial class PrihlaseniWindow : Window
   {
      /// <summary>
      /// Pomocná proměnná pro uchování zadaného jména uživatele
      /// </summary>
      private string Jmeno;

      /// <summary>
      /// Pomocná proměnná pro uchování zadaného hesla uživatele
      /// </summary>
      private string Heslo;

      /// <summary>
      /// Barva pro nastavení tlačítka. 
      /// Barva vytvořena dle bitových hodnot jednotlivých barevných složek.
      /// </summary>
      private SolidColorBrush MyColorTyrkys = new SolidColorBrush(Color.FromArgb(255, 90, 216, 188));

      /// <summary>
      /// Barva pro nastavení tlačítka. 
      /// Barva vytvořena dle bitových hodnot jednotlivých barevných složek.
      /// </summary>
      private SolidColorBrush MyClolorFialovoModra = new SolidColorBrush(Color.FromArgb(255, 50, 10, 165));

      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;

      /// <summary>
      /// Instance hlavního okna
      /// </summary>
      private MainWindow HlavniOkno;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;



      /// <summary>
      /// Konstruktor třídy pro inicializaci okeního formuláře a nastavení úvodního stavu včetně inicializace všech proměnných.
      /// </summary>
      public PrihlaseniWindow(MainWindow HlavniOkno, Spravce_Aplikace Spravce)
      {
         InitializeComponent();                                      // Inicializace okna pro přihlášení uživatele
         Jmeno = "";                                                 // Inicializace proměnné pro uložení jména
         Heslo = "";                                                 // Inicializace proměnné pro uložení hesla
         ZavrenoBezUlozeni = 1;                                      // Nastavení hodnoty příznakového bytu pro defaultní nastavení
         SpravaAplikace = Spravce;                                   // Inicializace správce aplikace
         this.HlavniOkno = HlavniOkno;                               // Inicializace interní instance hlavního okna
         ZobrazHesloButton.Content = "Zobrazit heslo";               // Nastavení kontextu na tlačítku
         ZobrazHesloButton.Background = MyColorTyrkys;               // Nastavení barvy tlačítka při úvodním zobrazení
         HesloUzivatelePasswordBox.Visibility = Visibility.Visible;  // Zobrazení bloku pro zadání hesla
         HesloUzivateleTextBlock.Visibility = Visibility.Hidden;     // Skrytí bloku pro zobrazení hesla    
      }



      /// <summary>
      /// Otevření okna pro registraci nového uživatele.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void RegistraceButton_Click(object sender, RoutedEventArgs e)
      {
         RegistraceWindow registraceWindow = new RegistraceWindow(this,SpravaAplikace);
         registraceWindow.ShowDialog();
      }

      /// <summary>
      /// Metoda obsluhující stisk tlačítka pro přihlášení.
      /// Provede kontrolu zda daný uživatel existuje a při splnění podmínek provede přihlášení uživatele do systému.
      /// Přihlášení uživatele je realizováno načtením dat ze souboru. Pouze data, která mají spojení s přihlášeným uživatelem.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrihlaseniButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Přihlášení uživatele do systému
            PrihlasUzivatele();
           
            // Zobrazení informativního okna o úšpěšném přihlášení uživatele do systému
            //MessageBox.Show("Přihlášení proběhlo úspěšně.", "Přihlášení", MessageBoxButton.OK, MessageBoxImage.Information);

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
      /// Uložení zadaného hesla do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZadaniHesla(object sender,RoutedEventArgs e)
      {
         // Uložení textu napsaného v PasswordBox do proměnné
         Heslo = HesloUzivatelePasswordBox.Password.ToString();         
      }

      /// <summary>
      /// Uložení zadaného jména do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZadaniJmena(object sender, TextChangedEventArgs e)
      {
         // Uložení textu napsaného v TextBox do proměnné
         Jmeno = JmenoUzivateleTextBox.Text.ToString();                 
      }

      /// <summary>
      /// Metoda obsluhující kliknutí na tlačítko pro Zobrazení/Skrytí hesla.
      /// Metoda nejprve zkontroluje zda bylo heslo zadáno a pokud ano, vyvolá pomocnou metodu pro zobrazení zadaného hesla, nebo pro zobrazení pole pro zadání hesla.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZobrazHesloButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // V případě že není zadání heslo, vyvolá se varovné okno s upozorněním
            if (!(Heslo.Length > 0))
               throw new ArgumentException("Zadejte heslo!");

            // Volání pomocné metody podle toho, jaké tlačítko je zobrazeno v okně aplikace
            if (ZobrazHesloButton.Content.ToString() == "Zobrazit heslo")
            {
               ZobrazHeslo();
               ZobrazHesloButton.Content = "Skrýt heslo";            // Nastavení kontextu na tlačítku
               ZobrazHesloButton.Background = MyClolorFialovoModra;  // Nastavení barvy tlačítka
            }
            else
            {
               SkrytHeslo();
               ZobrazHesloButton.Content = "Zobrazit heslo";         // Nastavení kontextu na tlačítku
               ZobrazHesloButton.Background = MyColorTyrkys;         // Nastavení barvy tlačítka
            }

         }
         catch (Exception ex)    // V případě vyjímky se objeví varovné okno
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }

      }


      /// <summary>
      /// Pomocná metoda pro zobrazení zadaného hesla.
      /// Metoda skryje pole pro zadání hesla a překryje jej textovým polem zobrazující zadané heslo.
      /// </summary>
      private void ZobrazHeslo()
      {
         HesloUzivateleTextBlock.Width = HesloUzivatelePasswordBox.Width;  // Nastavení stejné šířky pro optické překrytí bloku
         HesloUzivateleTextBlock.Text = Heslo;                             // Zobrazení hesla do textového bloku
         HesloUzivateleTextBlock.Visibility = Visibility.Visible;          // Nastavení viditelnosti bloku pro zobrazení hesla
         HesloUzivatelePasswordBox.Visibility = Visibility.Hidden;         // Skrytí pole pro zadávání hesla
      }

      /// <summary>
      /// Pomocná metoda pro skrytí textového pole zobrazující zadané heslo.
      /// Metoda skryje zobrazené heslo a obnový pole pro zadání hesla.
      /// </summary>
      private void SkrytHeslo()
      {
         HesloUzivateleTextBlock.Text = "";                                // Smazání hesla ze zobrazovacího pole
         HesloUzivatelePasswordBox.Visibility = Visibility.Visible;        // Nastavení viditelnosti bloku pro zadávání hesla
         HesloUzivateleTextBlock.Visibility = Visibility.Hidden;           // Skrytí textového bloku pro zobrazení hesla
      }

      /// <summary>
      /// Metoda volána z okna pro registraci uživatele. 
      /// Metoda automaticky vyplní údaje a přihlásí uživatele.
      /// </summary>
      /// <param name="Jmeno">Jméno uživatele</param>
      /// <param name="Heslo">Heslo uživatele</param>
      public void AutomatickePrihlaseniRegistrovanehoUzivatele(string Jmeno, string Heslo)
      {
         try
         {
            // Načtení údajů z parametru a provedení přihlášení uživatele
            this.Jmeno = Jmeno;
            this.Heslo = Heslo;
            PrihlasUzivatele();

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
      /// Metoda pro přihlášení uživatele do systému.
      /// </summary>
      private void PrihlasUzivatele()
      {
         // Kontrola zda je zadáno jméno
         if (!(Jmeno.Length > 0))
            throw new ArgumentException("Zadejte jméno!");

         // Kontrola zda je zadáno heslo
         if (!(Heslo.Length > 0))
            throw new ArgumentException("Zadejte heslo!");

         // Přihlášení uživatele do systému (nahrání jeho dat do interní kolkece pro zpracování)
         SpravaAplikace.PrihlaseniUzivatele(Jmeno, Heslo);

         // Předání uživatelského jména do hlavního okna
         HlavniOkno.JmenoPrihlasenehoUzivatele = Jmeno;

         // Načtení uživatelského rozhraní v hlavním oknu aplikace 
         HlavniOkno.NacteniPrihlasenehoUzivatele();
      }


      /// <summary>
      /// Zobrazení varovné zprávy při zavírání okna.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         // Pokud je okno zavřeno křížkem (bez přihlášení) zobrazí se varovné okno
         if (ZavrenoBezUlozeni == 1)
         {
            MessageBoxResult VybranaMoznost = MessageBox.Show("Nejste přihlášen! \nChcete vstoupit anonymě?", "Pozor", MessageBoxButton.YesNo, MessageBoxImage.Question);

            switch (VybranaMoznost)
            {
               case MessageBoxResult.Yes:
                  HlavniOkno.NeprihlasenyUzivatel();
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
