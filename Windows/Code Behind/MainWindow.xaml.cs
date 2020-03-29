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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using Calculator;
using System.Windows.Threading;
using System.ComponentModel;


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
   /// Třída obsluhující hlavní okenní formulář aplikace.
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře MainWindow.xaml
   /// Třída obsahuje potřebné metody a funkce pro chod hlavního okna aplikace včetně metod pro otevření a komunikaci s jinými okenními formuláři.
   /// </summary>
   public partial class MainWindow : Window 
   {
      /// <summary>
      /// Instance časovače pro vyvolání metody v určitém časovém intervalu.
      /// </summary>
      private DispatcherTimer CasovacSpousteniCasu;

      /// <summary>
      /// Pomocná proměnná pro kontrolu aktuálního času.
      /// Tato proměnná je využita pro zajištění, že se čas bude aktualizovat jen když dojde ke změně časové hodnoty (šetření výpočetního času).
      /// </summary>
      private int PomocneSekundy;

      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;

      /// <summary>
      /// Instance třídy pro vykreslování grafických prvků
      /// </summary>
      private Vykreslovani vykreslovani;

      /// <summary>
      /// Instance třídy pro práci s grafickou třídou Canvas
      /// </summary>
      private Canvas MyCanvas;

      /// <summary>
      /// Instance třídy pro vyhledávání konkrétních dat
      /// </summary>
      private Vyhledavani vyhledavani;

      /// <summary>
      /// Instance třídy pro převedení kolekce záznamů do grafické podoby
      /// </summary>
      private GrafickyZaznam grafickyZaznam;

      /// <summary>
      /// Vybraný záznam z úvodního zobrazení záznamů
      /// </summary>
      private Zaznam VybranyZaznam;

      /// <summary>
      /// Jméno přihlášeného uživatele
      /// </summary>
      public string JmenoPrihlasenehoUzivatele { get; set; }

      /// <summary>
      /// ValueTuple pro uchování rozměrů hlavního okna
      /// </summary>
      private (double Sirka, double Vyska) VelikostOkna;

      /// <summary>
      /// Příznakový bit informující zda je vykresleno levé postranní MENU
      /// </summary>
      private byte LeveMENU_Zobrazeno;

      /// <summary>
      /// Příznakový bit informující zda je vykresleno pravé postranní MENU
      /// </summary>
      private byte PraveMENU_Zobrazeno;

      /// <summary>
      /// Uložení barvy pro nastavení barvy pozadí
      /// </summary>
      public Brush BarvaPozadi { get; private set; }
  


      /// <summary>
      /// Konstruktor třídy pro inicializaci hlavního okna a úvodního nastavení aplikace.
      /// </summary>
      public MainWindow()
      {
         InitializeComponent();                                               // Inicializace hlavního okna
         BarvaPozadi = Brushes.LightBlue;                                     // Nastavení výchozí barvy pozadí
         PomocneSekundy = 0;                                                  // Nastavení úvodní hodnoty pomocné proměnné
         JmenoPrihlasenehoUzivatele = "";                                     // Inicializace textového řetězce
         LeveMENU_Zobrazeno = 0;                                              // Úvodní nastavení pro zobrazení levého postranního MENU
         PraveMENU_Zobrazeno = 0;                                             // Úvodní nastavení pro zobrazení pravého postranního MENU
         DataContext = SpravaAplikace;                                        // Nastavení kontextu okna pro možnost bindování hodnot 
         vykreslovani = new Vykreslovani(this);                               // Inicializace instance pro možnost využití grafických prvků
         MyCanvas = new Canvas();                                             // Inicializace instance pro možnost práce se třídou Canvas
         vyhledavani = new Vyhledavani();                                     // Inicializace instance pro možnost vyhledávat konkrétní data dle potřebných parametrů
         grafickyZaznam = new GrafickyZaznam(this, InformacniBublinaCanvas);  // Inicializace instance třídy pro grafické zpracování záznamů

         // Uložení rozměrů okna při inicializaci
         VelikostOkna = (HlavniOknoWindow.ActualWidth, HlavniOknoWindow.ActualHeight);

         try
         {
            // Inicializace časovače s nastavením spouštěního intervalu (vyvolá obsluhu události každých 20 ms)
            CasovacSpousteniCasu = new DispatcherTimer
            {
               Interval = TimeSpan.FromMilliseconds(20)
            };

            // Přiřazení obsluhy události časovači
            CasovacSpousteniCasu.Tick += CasovacSpousteniCasu_Tick;

            // Spuštění časovače
            CasovacSpousteniCasu.Start();

            // Vytvoření instance třídy pro správu aplikace s úvodním nastavením včetně načtení dat ze souboru
            SpravaAplikace = new Spravce_Aplikace();

            // Otevření okna pro přihlášení uživatele 
            PrihlaseniWindow Prihlaseni = new PrihlaseniWindow(this, SpravaAplikace);
            Prihlaseni.ShowDialog();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }



      /// <summary>
      /// Nastavení barvy pozadí.
      /// </summary>
      /// <param name="Barva">Zvolená barva</param>
      public void NastavBarvuPozadi(Brush Barva)
      {
         // Uložení zvolené barvy do interní proměnné a nastavení této barvy na pozadí
         BarvaPozadi = Barva;
         HlavniOknoGrid.Background = BarvaPozadi;

         // Aktualizace vykreslení informačního bloku pro aktualizaci barvy pozadí
         ZobrazBlokInformaciMesice();

         // Nastavení pozadí poznámkového bloku pokud je zobrazen
         if(PoznamkovyBlokStackPanel.Visibility == Visibility.Visible)
         {
            PoznamkovyBlokTextBox.Background = BarvaPozadi;
         }
      }

      /// <summary>
      /// Úvodní nastavení výchozího okna (vykreslení grafických prvků při výchozím zobrazení)
      /// </summary>
      public void UvodniNastaveni()
      {
         // Zobrazení seznamu záznamů při úvodním vykreslení
         ZobrazSeznamZaznamu();

         // Nastavení zobrazení poznámkového bloku
         NastavPoznamkovyBlok(SpravaAplikace.VratZobrazeniPoznamky());

         // Vykreslení postranních MENU se skrytými ovládacími prvky
         vykreslovani.SkryjLeveMENU(LeveMENU_Canvas);
         vykreslovani.SkryjPraveMENU(PraveMENU_Canvas);
      }

      /// <summary>
      /// Nalezení vybraného záznamu v kolekci dat přihlášeného uživatele na základě porovnání s předaným záznamem
      /// </summary>
      /// <param name="zaznam">Vybraný záznam</param>
      public void VyberZaznam(Zaznam zaznam)
      {
         foreach (Zaznam z in SpravaAplikace.KolekceDatPrihlasenehoUzivatele)
         {
            if (z.Equals(zaznam))
               VybranyZaznam = z;
         }
      }

      /// <summary>
      /// Zrušení označení vybraného záznamu pro zamezení nechtěného smazání
      /// </summary>
      public void ZrusOznaceniZaznamu()
      {
         VybranyZaznam = null;
      }

      /// <summary>
      /// Otevření okna pro úpravu vybraného záznamu
      /// </summary>
      public void UpravitZaznam()
      {
         try
         {
            UpravitZaznamWindow upravitZaznamWindow = new UpravitZaznamWindow(VybranyZaznam, SpravaAplikace);
            upravitZaznamWindow.ShowDialog();

            // Aktualizace vykresleného seznamu záznamů
            ZobrazSeznamZaznamu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         } 
      }

      /// <summary>
      /// Nastavení poznámkového bloku a předání textového bloku správci aplikace
      /// </summary>
      /// <param name="Zobrazit">Příznakový bit: 0 - nevykreslovat, 1 - vykreslit</param>
      public void NastavPoznamkovyBlok(byte Zobrazit)
      {
         // Pokud je poznámkový blok určen k zobrazení, provede se jeho inicializace
         if (Zobrazit == 1)
         {
            // Nastavení viditelnosti poznámkového bloku
            PoznamkovyBlokStackPanel.Visibility = Visibility.Visible;

            // Nastavení barvy poznámkového bloky dle zvolené barvy pozadí
            PoznamkovyBlokTextBox.Background = BarvaPozadi;
         }
         else
            PoznamkovyBlokStackPanel.Visibility = Visibility.Collapsed;

         // Předání parametrů správci aplikace pro možnost práce s poznámkovým blokem
         SpravaAplikace.NastavPoznamkuUzivatele(PoznamkovyBlokTextBox, Zobrazit);
      }

      /// <summary>
      /// Načtení dat přihlášeného uživatele a nastavení úvodních parametrů
      /// </summary>
      public void NacteniPrihlasenehoUzivatele()
      {
         // Kontrola zda je přihlášen správný uživatel
         if (SpravaAplikace.KontrolaPrihlaseniUzivatele(JmenoPrihlasenehoUzivatele))
         {
            // Povolení možnosti změnit velikost okna
            HlavniOknoWindow.ResizeMode = ResizeMode.CanResize;

            // Nastavení viditelnosti ovládacích prvků a skrytí plátna pro nepřihlášeného uživatele (včetně smazání obsahu plátna)
            NeprihlasenyUzivatel_Canvas.Children.Clear();
            NeprihlasenyUzivatel_Canvas.Visibility = Visibility.Collapsed;

            // Úvodní nastavení grafického rozvržení celé obrazovky
            UvodniNastaveni();
         }
         else
         {
            NeprihlasenyUzivatel();
         }
      }

      /// <summary>
      /// Metoda pro překrytí hlavního okna pro případ že není přihlášen žádný uživatel
      /// </summary>
      public void NeprihlasenyUzivatel()
      {
         // Smazání jména posledního příhlášeného uživatele (nepřihlášný uživatel nemá jméno)
         JmenoPrihlasenehoUzivatele = "";

         // Zakázání změnit velikost okna pro nepřihlášeného uživatele
         HlavniOknoWindow.ResizeMode = ResizeMode.NoResize;

         // Nastavení viditelnosti plátna pro nepřihlášeného uživatele a skrytí ovládacích prvků (včetně smazání obsahu plátna)
         NeprihlasenyUzivatel_Canvas.Children.Clear();
         NeprihlasenyUzivatel_Canvas.Visibility = Visibility.Visible;
         NeprihlasenyUzivatel_Canvas.Background = BarvaPozadi;
         vykreslovani.VykresleniObrazovkyProNeprihlasenehoUzivatele(NeprihlasenyUzivatel_Canvas, VelikostOkna.Sirka, VelikostOkna.Vyska);
      }


      /// <summary>
      /// Metoda pro řízení zobrazování aktuálního času. 
      /// Metoda se spoští v krátkých časových intervalech a při každém spuštění kontroluje zda došlo ke změně (aktualizaci) času. 
      /// Pokud se časová hodnota změnila je volána pomocná metoda pro aktualizaci hodnot zobrazujících aktuální čas.
      /// Dále je tato metoda využita pro kontrolu změny velikosti hlavního okna pro možnost reakce na danou změnu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void CasovacSpousteniCasu_Tick(object sender, EventArgs e)
      {
         // Pokud došlo ke změně času zavolá se metoda pro aktualizaci času zobrazovaného v okně aplikace
         if (DateTime.Now.Second != PomocneSekundy)
            AktualizaceDataCasu();

         // Uložení aktuálního času do pomocné proměnné
         PomocneSekundy = DateTime.Now.Second;


         // Kontrola zda byla změněna šířka nebo výška hlavního okna pro případ, že je vykresleno plátno pro nepřihlášeného uživatele.
         // Pokud dojde ke změně velikoti okna obnoví se vykreslení plátna za účelem aktualizace velikosti plátna aby plátno bylo stále vykresleno přes celé okno
         // Dále se uloží nový rozměr okna do interní proměnné
         if (!(VelikostOkna.Sirka == HlavniOknoWindow.ActualWidth || VelikostOkna.Vyska == HlavniOknoWindow.ActualHeight) && NeprihlasenyUzivatel_Canvas.Visibility == Visibility.Visible)
         {
            VelikostOkna.Sirka = HlavniOknoWindow.ActualWidth;
            VelikostOkna.Vyska = HlavniOknoWindow.ActualHeight;
            NeprihlasenyUzivatel();
         }
      }


      /// <summary>
      /// Pomocná metoda pro aktualizaci dat zobrazující aktuální čas v okenním formuláři aplikace.
      /// </summary>
      private void AktualizaceDataCasu()
      {
         // Vytvoření instance pomocné třídy pro řízení zobrazovaných hodin
         Hodiny hodiny = new Hodiny();

         // Aktualizace digitálního zobrazení data a času
         DigitalniHodinyLabel.Content = hodiny.VratAktualniCas(0);
         DatumLabel.Content = hodiny.VratAktualniDatum();

         // Aktualizace polohy ručiček analogových hodin
         VterinovaRucickaRotace.Angle = hodiny.NastavUhelVterinoveRucicky();
         MinutovaRucickaRotace.Angle = hodiny.NastavUhelMinutoveRucicky();
         HodinovaRucickaRotace.Angle = hodiny.NastavUhelHodinoveRucicky();
      }

      /// <summary>
      /// Vykreslení seznamu záznamů aktuálního měsíce do úvodního okna
      /// </summary>
      private void ZobrazSeznamZaznamu()
      {
         // Zrušení označení vybraného záznamu
         VybranyZaznam = null;

         // Smazání obsahu plátna pro vykreslení záznamů
         UvodniSeznamZaznamuCanvas.Children.Clear();

         // Načtení kolekce dat ke grafickému zpracování
         grafickyZaznam.ObnovKolekciZaznamu(vyhledavani.VratZaznamyAktualnihoMesice(SpravaAplikace.KolekceDatPrihlasenehoUzivatele));

         // Vykreslení zpracovaného seznamu záznamů do úvodního okna aplikace
         vykreslovani.VykresliPrvek(UvodniSeznamZaznamuCanvas, grafickyZaznam.StrankaZaznamu);

         // Aktualizace informačního bloku na úvodní obrazovce
         ZobrazBlokInformaciMesice();
      }

      /// <summary>
      /// Zobrazení informačního bloku o zobrazených záznamech
      /// </summary>
      private void ZobrazBlokInformaciMesice()
      {
         // Vykreslení informačního bloku o zázname aktuálního měsíce
         vykreslovani.InformacniBlokHlavnihoOkna(PrehledCanvas, BarvaPozadi, vyhledavani.VratZaznamyAktualnihoMesice(SpravaAplikace.KolekceDatPrihlasenehoUzivatele));
      }



      /// <summary>
      /// Otevření okna pro přidání nového záznamu.
      /// Po zavření vyvolaného okna se aktualizuje zobrazený seznam.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PridatZaznam_Click(object sender, RoutedEventArgs e)
      {
         PridatZaznamWindow pridatZaznamWindow = new PridatZaznamWindow(SpravaAplikace, BarvaPozadi);
         pridatZaznamWindow.ShowDialog();

         // Aktualizace vykresleného seznamu záznamů
         ZobrazSeznamZaznamu();
      }

      /// <summary>
      /// Metoda pro smazání vybraného záznamu.
      /// Metoda je vyvolána stiskem tlačítka pro odebrání záznamu.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void OdebratZaznam_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Kontrola zda byl vybrán záznam pro odstranění
            if (VybranyZaznam == null)
               throw new ArgumentException("Vyberte záznam!");

            // Zobrazení varovného okna s načtením zvolené volby
            MessageBoxResult VybranaVolba = MessageBox.Show("Opravdu chcete vybraný záznam odstranit?", "Upozornění", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Smazání vybrané položky v případě stisku tlačíka YES
            switch (VybranaVolba)
            {
               case MessageBoxResult.Yes:
                  SpravaAplikace.KolekceDatPrihlasenehoUzivatele.Remove(VybranyZaznam);   // Smazání vybraného záznamu z kolekce dat
                  VybranyZaznam = null;                                                   // Smazání reference na vymazanou položku
                  ZobrazSeznamZaznamu();                                                  // Aktualizace vykresleného seznamu
                  break;

               case MessageBoxResult.No:
                  break;
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Otevření okna pro možnost vyhledávání záznamů dle zvolených kritérií.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Vyhledat_Click(object sender, RoutedEventArgs e)
      {
         // Zrušení označení záznamu pro možnost označit záznam v okně pro vyhledávaní
         VybranyZaznam = null;

         VyhledatWindow vyhledatWindow = new VyhledatWindow(SpravaAplikace.KolekceDatPrihlasenehoUzivatele, SpravaAplikace, this, null, BarvaPozadi);
         vyhledatWindow.ShowDialog();

         // Aktualizace vykresleného seznamu záznamů
         ZobrazSeznamZaznamu();
      }

      /// <summary>
      /// Otevření okna pro možnost prohlížení veškerých záznamů z kategorie Příjmy.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void ZobrazPrijmy_Click(object sender, RoutedEventArgs e)
      {
         PrijmyVydajeWindow prijmyVydajeWindow = new PrijmyVydajeWindow(1, vyhledavani.VratPrijmy(SpravaAplikace.KolekceDatPrihlasenehoUzivatele), SpravaAplikace, this, BarvaPozadi);
         prijmyVydajeWindow.ShowDialog();

         // Aktualizace vykresleného seznamu záznamů
         ZobrazSeznamZaznamu();
      }

      /// <summary>
      /// Otevření okna pro možnost prohlížení veškerých záznamů z kategorie Výdaje.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void ZobrazVydaje_Click(object sender, RoutedEventArgs e)
      {
         PrijmyVydajeWindow prijmyVydajeWindow = new PrijmyVydajeWindow(0, vyhledavani.VratVydaje(SpravaAplikace.KolekceDatPrihlasenehoUzivatele), SpravaAplikace, this, BarvaPozadi);
         prijmyVydajeWindow.ShowDialog();

         // Aktualizace vykresleného seznamu záznamů
         ZobrazSeznamZaznamu();
      }

      /// <summary>
      /// Otevření okna pro zobrazení statisticky zpracovaných dat (záznamů).
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Statistika_Click(object sender, RoutedEventArgs e)
      {
         StatistikaWindow statistikaWindow = new StatistikaWindow(SpravaAplikace.KolekceDatPrihlasenehoUzivatele, BarvaPozadi);
         statistikaWindow.ShowDialog();

         // Aktualizace vykresleného seznamu záznamů
         ZobrazSeznamZaznamu();
      }


      /// <summary>
      /// Metoda obsluhující tlačítko Kalkulacka.
      /// Otevření projektu z jiného jmeného prostoru. 
      /// Po kliknutí na tlačítko se otevře projekt realizující jednoduchou kalkulačku ve WPF.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void KalkulackaButton_Click(object sender, RoutedEventArgs e)
      {
         // Vytvoření instance projektu Calculator a otevření okna s kalkulačkou
         Calculator.MainWindow Kalkulacka = new Calculator.MainWindow();
         Kalkulacka.ShowDialog();
      }

      /// <summary>
      /// Otevření okenního formuláře pro možnost importovat data ze souboru.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void ImportDat_Click(object sender, RoutedEventArgs e)
      {
         ImportDatWindow importDatWindow = new ImportDatWindow(SpravaAplikace, this);
         importDatWindow.ShowDialog();
      }

      /// <summary>
      /// Otevření okenního formuláře pro možnost exportovat data do souboru (uložení záznamů do textového souboru).
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void ExportDat_Click(object sender, RoutedEventArgs e)
      {
         ExportDatWindow exportDatWindow = new ExportDatWindow(SpravaAplikace, SpravaAplikace.KolekceDatPrihlasenehoUzivatele, this);
         exportDatWindow.ShowDialog();
      }

      /// <summary>
      /// Uložení dat v aplikaci do souboru.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Ulozit_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            SpravaAplikace.UlozeniDatDoSouboru();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      /// <summary>
      /// Otevření okenního formuláře pro možnost změnit nastavení určitých vlastností aplikace.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Nastaveni_Click(object sender, RoutedEventArgs e)
      {
         NastaveniWindow nastaveniWindow = new NastaveniWindow(SpravaAplikace, this);
         nastaveniWindow.ShowDialog();

         // Aktualizace vykresleného seznamu záznamů
         ZobrazSeznamZaznamu();
      }

      /// <summary>
      /// Zobrazení informačního okna při kliknutí na příslušné tlačítko
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void InformaceButton_Click(object sender, RoutedEventArgs e)
      {
         InformacniWindow informacniWindow = new InformacniWindow(BarvaPozadi);
         informacniWindow.ShowDialog();
      }

      /// <summary>
      /// Obsluha události pro odhlášení aktuálního uživatele a vykreslení obrazovky pro nepřihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void Odhlasit_Click(object sender, RoutedEventArgs e)
      {
         SpravaAplikace.OdhlaseniUzivatele();
         NeprihlasenyUzivatel();
      }

      /// <summary>
      /// Metoda obsluhující tlačítko pro přihlášení.
      /// Tlačítko je umístěno v obrazovce pro nepřihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PrihlasitButton_Click(object sender, RoutedEventArgs e)
      {
         PrihlaseniWindow Prihlaseni = new PrihlaseniWindow(this, SpravaAplikace);
         Prihlaseni.ShowDialog();
      }

      /// <summary>
      /// Metoda pro otevření okna pro registraci nového uživatele.
      /// Tlačítko je umístěno v obrazovce pro nepřihlášeného uživatele.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void RegistrovatButton_Click(object sender, RoutedEventArgs e)
      {
         PrihlaseniWindow Prihlaseni = new PrihlaseniWindow(this, SpravaAplikace);
         Prihlaseni.RegistraceButton_Click(sender, e);
      }



      /// <summary>
      /// Obsluha události při najetí kurzoru myši na daný blok.
      /// Vykreslení postranního MENU v plné velikosti včetně ovládacích prvků.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void LeveMENU_MouseMove(object sender, MouseEventArgs e)
      {
         // Zabezpečení proti opakovanému vykreslování (kurzor jednou najede na daný blok, příznakový bit se přepne)
         if (LeveMENU_Zobrazeno == 0)
         {
            // Volání metody pro překreslení plátna 
            vykreslovani.VykresliLeveMENU(LeveMENU_Canvas);

            // Nastavení příznakového bitu
            LeveMENU_Zobrazeno = 1;
         }
      }

      /// <summary>
      /// Obsluha události při opuštění daného bloku kurzorem myši. 
      /// Vykreslené postranní MENU se schová do postranní lišty.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void LeveMENU_MouseLeave(object sender, MouseEventArgs e)
      {
         // Zabezpečení proti opakovanému vykreslování (kurzor jednou odjede, příznakový bit se přepne)
         if (LeveMENU_Zobrazeno == 1)
         {
            // Volání metody pro překreslení plátna 
            vykreslovani.SkryjLeveMENU(LeveMENU_Canvas);

            // Nastavení příznakového bitu
            LeveMENU_Zobrazeno = 0;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na daný blok.
      /// Vykreslení postranního MENU v plné velikosti včetně ovládacích prvků.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PraveMENU_MouseMove(object sender, MouseEventArgs e)
      {
         // Zabezpečení proti opakovanému vykreslování (kurzor jednou najede na daný blok, příznakový bit se přepne)
         if (PraveMENU_Zobrazeno == 0)
         {
            // Volání metody pro překreslení plátna 
            vykreslovani.VykresliPraveMENU(PraveMENU_Canvas);

            // Nastavení příznakového bitu
            PraveMENU_Zobrazeno = 1;
         }
      }

      /// <summary>
      /// Obsluha události při opuštění daného bloku kurzorem myši. 
      /// Vykreslené postranní MENU se schová do postranní lišty.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      public void PraveMENU_MouseLeave(object sender, MouseEventArgs e)
      {
         // Zabezpečení proti opakovanému vykreslování (kurzor jednou odjede, příznakový bit se přepne)
         if (PraveMENU_Zobrazeno == 1)
         {
            // Volání metody pro překreslení plátna 
            vykreslovani.SkryjPraveMENU(PraveMENU_Canvas);

            // Nastavení příznakového bitu
            PraveMENU_Zobrazeno = 0;
         }
      }



      /// <summary>
      /// Při zavření hlavního okna aplikace provede odhlášení uživatele (uložení dat do souboru).
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Window_Closing(object sender, CancelEventArgs e)
      {
         try
         {
            // Pokud je do aplikace přihlášen nějaý uživatel, provede se při zavírání odhlášení uživatele včetně uložení provedených změn 
            if (JmenoPrihlasenehoUzivatele.Length > 0)
               SpravaAplikace.OdhlaseniUzivatele();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }
      
   }
}
