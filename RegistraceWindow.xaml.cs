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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře RegistraceWindow.xaml
   /// Třída obsluhuje okenní formulář pro registraci nového uživatele.
   /// </summary>
   public partial class RegistraceWindow : Window
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
      /// Pomocný textový řetězec pro informování uživatele v případě nesplnění bezpečnostních podmínek při zadávání hesla.
      /// </summary>
      private string BezpecnostniZprava = "";

      /// <summary>
      /// Pomocná proměnná pro zobrazování síly hesla
      /// </summary>
      private int PocetSplnenychPodminekHesla;

      /// <summary>
      /// Instance třídy pro vykreslování grafických prvků
      /// </summary>
      private Vykreslovani VykreslovaciPrvek;

      /// <summary>
      /// Instance přihlašovacího okna
      /// </summary>
      private PrihlaseniWindow oknoPrihlaseni;

      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;



      /// <summary>
      /// Konstruktor třídy pro okenní formulář
      /// </summary>
      public RegistraceWindow(PrihlaseniWindow oknoPrihlaseni, Spravce_Aplikace Spravce)
      {
         InitializeComponent();                                            // Inicializace okenného formuláře
         Jmeno = "";                                                       // Inicializace proměnné pro uložení jména
         Heslo = "";                                                       // Inicializace proměnné pro uložení hesla
         this.oknoPrihlaseni = oknoPrihlaseni;                             // Inicializace instance přihlašovacího okna
         SpravaAplikace = Spravce;                                         // Inicializace správce aplikace
         VykreslovaciPrvek = new Vykreslovani();                           // Inicializace instance třídy pro vykreslování do okenního formuláře
         ZavrenoBezUlozeni = 1;                                            // Nastavení hodnoty příznakového bytu pro defaultní nastavení
         PocetSplnenychPodminekHesla = 0;                                  // Inicializace pomocné proměnné pro zobrazení síly hesla
         UkazatelSilyHeslaStackPanel.Visibility = Visibility.Collapsed;    // Skrytí ukazatele síly hesla
      }



      /// <summary>
      /// Pomocná metoda pro kontrolu zadaných údajů při vytváření nového uživatele
      /// </summary>
      private void KontrolaZadanychDat()
      {
         // Kontrola zda bylo zadáno jméno
         if (!(Jmeno.Length > 2))
            throw new ArgumentException("Zadejte jméno! (alespoň 3 znaky)");

         // Kontrola zda bylo zadáno heslo
         if (!(Heslo.Length > 0))
            throw new ArgumentException("Zadejte heslo!");

         // Kontrola bezpečnosti hesla
         if (!KontrolaMinimalniBezpecnostiHesla(Heslo))
            throw new ArgumentException("Zadané heslo nesplňuje bezpečnostní požadavky! " + BezpecnostniZprava);
      }

      /// <summary>
      /// Pomocná metoda pro kontrolu zda zadané helso splňuje minimální požadavky na bezpečnost.
      /// V metodě je kontrolováno celkem 6 bezpečnostních prvků a vrací TRUE pokud heslo splňuje alespoň nastavený minimální počet prvků (zde min 2 prvky).
      /// </summary>
      /// <returns>TRUE/FALSE vyhodnocení bezpečnosti hesla</returns>
      private bool KontrolaMinimalniBezpecnostiHesla(string heslo)
      {
         // Smazání obsahu bezpečnostní zprávy
         BezpecnostniZprava = "\n";

         // Pomocné proměnné pro kontrolu hesla:
         ///////////////////////////////////////
         string KontrolniHeslo = heslo;                                 // Uložení zadaného hesla do pomocného řetězce pro analýzu bezpečnosti
         const int MaximalniPocetNedostatku = 4;                        // Maximální počet nesplňených bezpečnostních prvků
         int PocetNedostatku = 0;                                       // Pomocná proměnná pro zjištění počtu nedostatků v zadaném hesle
         string Pismena = "abcdefghijklmnopqrstuvwxyz";                 // Pomocný řetězec pro kontrolu písmen
         string Cislice = "0123456789";                                 // Pomocný řetězec pro kontrolu číslic
         string SpecialniZnaky = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ "; // Pomocný řetězec pro kontorlu speciálních znaků
         string PismenaSDiakritikou = "ěščřžýáíéúůťďňó";                // Pomocný řetězec pro kontorlu písmen s diakritikou
         int PocetVelkychPismen = 0;                                    // Pomocný čítač velkých písmen 
         int PocetMalychPismen = 0;                                     // Pomocný čítač malých písmen
         int PocetCislic = 0;                                           // Pomocný čítač číslic
         int PocetSpecialnichZnaku = 0;                                 // Pomocný čítač speciálních znaků
         int PocetPismenSDiakritikou = 0;                               // Pomocný čítač písmen s diakritikou



         // Zjišťování splnění jednotlivých bezpečnostních požadavků:
         ////////////////////////////////////////////////////////////

         // Kontrola zda heslo obsahuje alespoň 1 malé písmeno
         foreach (char pismeno in Pismena)
         {
            PocetMalychPismen += KontrolniHeslo.Contains(pismeno.ToString()) ? 1 : 0;
         }

         // Kontrola zda heslo obsahuje alespoň 1 velké písmeno
         foreach (char pismeno in Pismena)
         {
            PocetVelkychPismen += KontrolniHeslo.Contains(pismeno.ToString().ToUpper()) ? 1 : 0;
         }

         // Kontrola zda heslo obsahuje alespoň 1 číslici
         foreach (char cislice in Cislice)
         {
            PocetCislic += KontrolniHeslo.Contains(cislice.ToString()) ? 1 : 0;
         }

         // Kontrola zda heslo obsahuje alespoň 1 speciální znak
         foreach (char znak in SpecialniZnaky)
         {
            PocetSpecialnichZnaku += KontrolniHeslo.Contains(znak.ToString()) ? 1 : 0;
         }

         // Kontrola zda heslo obsahuje alespoň 1 písmeno s diakritikou (malé nebo velké)
         foreach (char pismeno in PismenaSDiakritikou)
         {
            PocetPismenSDiakritikou += (KontrolniHeslo.Contains(pismeno.ToString()) || KontrolniHeslo.Contains(pismeno.ToString().ToUpper())) ? 1 : 0;
         }



         // Kontrolní podmínky pro zjištění nedostatků v zadaném hesle:
         //////////////////////////////////////////////////////////////

         // Minimální délka hesla
         if (KontrolniHeslo.Length < 5)
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo nesplňuje minimání délku. \n";
         }

         // Pokud není počet malých písmen v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetMalychPismen > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje malé písmeno. \n";
         }

         // Pokud není počet velkých písmen v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetVelkychPismen > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje velké písmeno. \n";
         }

         // Pokud není počet číslic v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetCislic > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje číslici. \n";
         }

         // Pokud není počet speciálních znaků v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetSpecialnichZnaku > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje speciální znak. \n";
         }

         // Pokud není počet písmen s diakritikou v hesle větší než 0, inkrementuje se čítač počtu nedostatků
         if (!(PocetPismenSDiakritikou > 0))
         {
            PocetNedostatku++;
            BezpecnostniZprava += "Heslo neobsahuje písmeno s diakritikou. \n";
         }

         // Nastavení pomocné proměné počtu splněných podmínek pro určení síly hesla
         PocetSplnenychPodminekHesla = 6 - PocetNedostatku;

         // Návratová hodnota nastavena podle toho, zda zadané heslo splňuje minimální bezpečnostní nastavení
         return PocetNedostatku > MaximalniPocetNedostatku ? false : true;
      }

      /// <summary>
      /// Metoda pro nastavení ukazatele síly hesla.
      /// Nastaví viditelnost prvku podle toho, zda je nějaké heslo zadáno. Následně upraví grafické zobrazení na základě vyhodnocení bezpečnosti hesla.
      /// </summary>
      private void UkazatelHesla()
      {
         // Pokud není heslo zadáno ukazatel síly hesla zůstane skrytý
         UkazatelSilyHeslaStackPanel.Visibility = Heslo.Length > 0 ? Visibility.Visible : Visibility.Hidden;

         // Volání metody pro kontrolu bezpečnosti hesla
         KontrolaMinimalniBezpecnostiHesla(Heslo);

         // Vykreslení ukazatele síly hesla do canvasu
         VykreslovaciPrvek.VykresleniUkazateleHesla(UkazatelSilyHeslaCanvas, PocetSplnenychPodminekHesla, 6, 200, 20);
      }


      /// <summary>
      /// Obslužná metoda stisku tlačítka pro registraci uživatele.
      /// V případě splnění podmínek provede registraci nového uživatele do systému a zavře okenní formulář.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void RegistraceButton_Click(object sender, RoutedEventArgs e)
      {
         // Registrace uživatele na základě zadaných údajů
         try
         {
            // Kotrola zadaných dat od uživatele
            KontrolaZadanychDat();

            // Kontrola zda již neexistuje uživatel se stejným jménem a heslem a následné vytvoření nově registrovaného uživatele
            SpravaAplikace.VytvoreniNovehoUzivatele(Jmeno, Heslo);

            // Automatické přihlášení nově registrovaného uživatele
            oknoPrihlaseni.AutomatickePrihlaseniRegistrovanehoUzivatele(Jmeno, Heslo);

            // Zobrazení informativního okna o úšpěšném provedení registrace nového uživatele do systému
            MessageBox.Show("Nový uživatel byl registrován.", "Registrace proběhla úspěšně", MessageBoxButton.OK, MessageBoxImage.Information);

            // Zavření okna
            ZavrenoBezUlozeni = 0;
            Close();
         }
         catch (Exception ex)    // Zobrazení varování v případě chyby
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Uložení zadaného hesla do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void ZadaniHesla(object sender, RoutedEventArgs e)
      {
         // Uložení textu napsaného v PasswordBox do proměnné
         Heslo = HesloUzivatelePasswordBox.Password.ToString();

         // Zobrazení ukazatele síly zadaného hesla
         UkazatelHesla();
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
      /// Zobrazení varovné zprávy při zavírání okna
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
