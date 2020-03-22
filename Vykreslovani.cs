using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Serialization;
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
   /// Třída pro obsluhu grafických prvků. 
   /// Slouží k vykreslování potřebných údajů na plátno a vytváření graficky reprezentovaných údajů.
   /// Třída obsahuje obecné metody pro vykreslení předaných prvků na plátno. Dále obsahuje metody pro grafické zpracování funkcí (bloků) v hlavním okně.
   /// Pro použití metod určených pro práci s hlavním oknem je třeba využít druhý konstruktor při vytváření instance této třídy.
   /// </summary>
   public class Vykreslovani
   {
      /// <summary>
      /// Instance hlavního okna aplikace pro možnost komunikace s ní.
      /// </summary>
      private MainWindow HlavniOkno;

      /// <summary>
      /// Rozměry tlačítek
      /// </summary>
      public (int Sirka, int Vyska, int Mezera) RozmeryButton { get; private set; }

      /// <summary>
      /// Instance třídy pro správu data a času
      /// </summary>
      private Hodiny hodiny;



      /// <summary>
      /// Konstruktor třídy
      /// </summary>
      public Vykreslovani()
      {
         RozmeryButton = (100, 30, 20);    // Úvodní nastavení rozměrů tlačítek
         hodiny = new Hodiny();           // Vytvoření instance třídy pro správu data a času
      }

      /// <summary>
      /// Konstruktor třídy pro možnost vytvoření této instance z hlavního okna aplikace
      /// </summary>
      /// <param name="HlavniOkno">Instance hlavního okna</param>
      public Vykreslovani(MainWindow HlavniOkno)
      {
         this.HlavniOkno = HlavniOkno;
         RozmeryButton = (100, 30, 15);    // Úvodní nastavení rozměrů tlačítek
         hodiny = new Hodiny();           // Vytvoření instance třídy pro správu data a času
      }



      /// <summary>
      /// Vykreslení plátna pro anonymní mód, tedy pro nepřihlášeného uživatele.
      /// Metody vykreslí pouze omezený počet ovládacích prvků.
      /// </summary>
      /// <param name="Mycanvas">Vykreslovací plátno z okenního formuláře</param>
      /// <param name="sirka">Šířka vykreslovaného plátna</param>
      /// <param name="vyska">Výška vykreslovaného plátna</param>
      public void VykresleniObrazovkyProNeprihlasenehoUzivatele(Canvas Mycanvas, double sirka, double vyska)
      {
         // Vytvoření pozadí pro plné vyplnění plátna přes celé okno
         Rectangle Pozadi = new Rectangle
         {
            Height = vyska,
            Width = sirka
         };

         // Přidání pozadí mezi potomky -> vykreslení na canvas
         Mycanvas.Children.Add(Pozadi);
         Canvas.SetLeft(Pozadi, 0);
         Canvas.SetTop(Pozadi, 0);
         //////////////////////////////////////////////////////////////////////////

         // Vytvoření tlačítka pro možnost přihlášení uživatele
         Button PrihlasitButton = new Button
         {
            Width = sirka / 5,
            Height = vyska / 5,
            Content = "Přihlásit se",
            FontSize = 24,
            Background = Brushes.Green
         };
         
         // Přidání události pro možnost obsluhy při kliknutí na tlačítko
         PrihlasitButton.Click += HlavniOkno.PrihlasitButton_Click;

         // Přidání tlačítka mezi potomky -> vykreslení na canvas
         Mycanvas.Children.Add(PrihlasitButton);
         Canvas.SetLeft(PrihlasitButton, sirka / 2 - PrihlasitButton.Width / 2);
         Canvas.SetTop(PrihlasitButton, vyska / 2 - PrihlasitButton.Height / 2);
         //////////////////////////////////////////////////////////////////////////

         // Vytvoření tlačítka pro možnost registrace nového uživatele
         Button RegistraceButton = new Button
         {
            Width = 130,
            Height = 30,
            FontSize = 16,
            Content = "Registrovat se",
            Background = Brushes.OrangeRed
         };

         // Přidání události pro možnost obsluhy při kliknutí na tlačítko
         RegistraceButton.Click += HlavniOkno.RegistrovatButton_Click;

         // Přidání tlačítka mezi potomky -> vykreslení na canvas
         Mycanvas.Children.Add(RegistraceButton);
         Canvas.SetLeft(RegistraceButton, 20);
         Canvas.SetTop(RegistraceButton, vyska - 3 * RegistraceButton.Height);
         //////////////////////////////////////////////////////////////////////////

         // Vytvoření tlačítka pro zobrazení informačního okna
         Button InfoButton = new Button
         {
            Width = 70,
            Height = 30,
            FontSize = 18,
            FontStyle = FontStyles.Italic,
            Content = "Info",
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Background = Brushes.DarkSalmon
         };

         // Přidání události pro možnost obsluhy při kliknutí na tlačítko
         InfoButton.Click += HlavniOkno.InformaceButton_Click;

         // Přidání tlačítka mezi potomky -> vykreslení na canvas
         Mycanvas.Children.Add(InfoButton);
         Canvas.SetLeft(InfoButton, sirka - InfoButton.Width - 40);
         Canvas.SetTop(InfoButton, vyska - 3 * InfoButton.Height);
         //////////////////////////////////////////////////////////////////////////
      }


      /// <summary>
      /// Vykreslení předaného prvku na předaný canvas
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení</param>
      /// <param name="Prvek">Prvek určený k vykreslení</param>
      public void VykresliPrvek(Canvas MyCanvas, StackPanel Prvek)
      {
         MyCanvas.Children.Add(Prvek);
         Canvas.SetLeft(Prvek, 0);
         Canvas.SetTop(Prvek, 0);
      }


      /// <summary>
      /// Metoda pro vykreslení ovládacích prvků do levého postranního MENU.
      /// </summary>
      /// <param name="LeveMENU">Plátno pro vykreslení</param>
      public void VykresliLeveMENU(Canvas LeveMENU)
      {
         // Smazání obsahu plátna
         LeveMENU.Children.Clear();

         // Přidání události pro možnost skrytí MENU
         LeveMENU.MouseLeave += HlavniOkno.LeveMENU_MouseLeave;

         // Vytvoření obdélníku vykreslení pozadí levého MENU
         Rectangle rectangle = new Rectangle
         {
            Fill = Brushes.Green,
            Width = 120,
            Height = HlavniOkno.Height
         };

         // Přidání pozadí na plátno
         LeveMENU.Children.Add(rectangle);
         Canvas.SetLeft(rectangle, 0);
         Canvas.SetTop(rectangle, 0);

         // Vytvoření bloku pro vykreslení jména uživatele
         StackPanel prihlasenyUzivatel = new StackPanel
         {
            Orientation = Orientation.Vertical
         };

         // Titulek pro jméno uživatele
         Label titulek = new Label
         {
            Content = "Přihlášený uživatel: ",
            FontSize = 12,
            HorizontalContentAlignment = HorizontalAlignment.Center
         };

         // Jméno uživatele
         Label jmeno = new Label
         {
            Content = HlavniOkno.JmenoPrihlasenehoUzivatele,
            FontSize = 16,
            Foreground = Brushes.LightGoldenrodYellow,
            FontWeight = FontWeights.Bold,
            HorizontalContentAlignment = HorizontalAlignment.Center
         };

         // Přidání prvků do bloku a následné vykreslení na plátno
         prihlasenyUzivatel.Children.Add(titulek);
         prihlasenyUzivatel.Children.Add(jmeno);
         LeveMENU.Children.Add(prihlasenyUzivatel);
         Canvas.SetLeft(prihlasenyUzivatel, 0);
         Canvas.SetTop(prihlasenyUzivatel, 5);




         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro přidání nového záznamu
         /// /////////////////////////////////

         // Vytvoření tlačítka Pridat
         Button Pridat = new Button
         {
            Background = Brushes.DarkSalmon,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Přidat",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Pridat.Click += HlavniOkno.PridatZaznam_Click;

         // Přidání tlačítka Pridat na plátno
         LeveMENU.Children.Add(Pridat);
         Canvas.SetLeft(Pridat, 10);
         Canvas.SetTop(Pridat, 100);



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro odebrání vybraného záznamu
         /// ///////////////////////////////////////

         // Vytvoření tlačítka Odebrat
         Button Odebrat = new Button
         {
            Background = Brushes.Red,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Odebrat",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Odebrat.Click += HlavniOkno.OdebratZaznam_Click;

         // Přidání tlačítka Odebrat na plátno
         LeveMENU.Children.Add(Odebrat);
         Canvas.SetLeft(Odebrat, 10);
         Canvas.SetTop(Odebrat, 100 + (RozmeryButton.Vyska + RozmeryButton.Mezera));



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro vyhledávání mezi záznamy
         /// /////////////////////////////////////

         // Vytvoření tlačítka Vyhledat
         Button Vyhledat = new Button
         {
            Background = Brushes.DarkSalmon,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Vyhledat",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Vyhledat.Click += HlavniOkno.Vyhledat_Click;

         // Přidání tlačítka Vyhledat na plátno
         LeveMENU.Children.Add(Vyhledat);
         Canvas.SetLeft(Vyhledat, 10);
         Canvas.SetTop(Vyhledat, 100 + (RozmeryButton.Vyska + RozmeryButton.Mezera) + (RozmeryButton.Vyska + 3 * RozmeryButton.Mezera));



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro zobrazení příjmů
         /// /////////////////////////////

         // Vytvoření tlačítka Prijem
         Button Prijem = new Button
         {
            Background = Brushes.LimeGreen,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Příjmy",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Prijem.Click += HlavniOkno.ZobrazPrijmy_Click;

         // Přidání tlačítka Prijem na plátno
         LeveMENU.Children.Add(Prijem);
         Canvas.SetLeft(Prijem, 10);
         Canvas.SetTop(Prijem, 100 + (RozmeryButton.Vyska + RozmeryButton.Mezera) * 2 + (RozmeryButton.Vyska + 5 * RozmeryButton.Mezera));



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro zobrazení výdajů
         /// /////////////////////////////

         // Vytvoření tlačítka Vydaj
         Button Vydaj = new Button
         {
            Background = Brushes.Crimson,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Výdaje",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Vydaj.Click += HlavniOkno.ZobrazVydaje_Click;

         // Přidání tlačítka Vydaj na plátno
         LeveMENU.Children.Add(Vydaj);
         Canvas.SetLeft(Vydaj, 10);
         Canvas.SetTop(Vydaj, 100 + (RozmeryButton.Vyska + RozmeryButton.Mezera) * 3 + (RozmeryButton.Vyska + 5 * RozmeryButton.Mezera));



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro zobrazení statistiky
         /// /////////////////////////////////

         // Vytvoření tlačítka Statistika
         Button Statistika = new Button
         {
            Background = Brushes.DarkTurquoise,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Statistika",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Statistika.Click += HlavniOkno.Statistika_Click;

         // Přidání tlačítka Statistika na plátno
         LeveMENU.Children.Add(Statistika);
         Canvas.SetLeft(Statistika, 10);
         Canvas.SetTop(Statistika, 100 + (RozmeryButton.Vyska + RozmeryButton.Mezera) * 4 + (RozmeryButton.Vyska + 6 * RozmeryButton.Mezera));

      }


      /// <summary>
      /// Metoda pro vykreslení ovládacích prvků do pravého postranního MENU.
      /// </summary>
      /// <param name="PraveMENU">Plátno pro vykreslení</param>
      public void VykresliPraveMENU(Canvas PraveMENU)
      {
         // Smazání obsahu plátna
         PraveMENU.Children.Clear();

         // Přidání události pro možnost skrytí MENU
         PraveMENU.MouseLeave += HlavniOkno.PraveMENU_MouseLeave;

         // Vytvoření obdélníku vykreslení pozadí pravého MENU
         Rectangle rectangle = new Rectangle
         {
            Fill = Brushes.Green,
            Width = 120,
            Height = HlavniOkno.Height
         };

         // Přidání pozadí na plátno
         PraveMENU.Children.Add(rectangle);
         Canvas.SetRight(rectangle, 0);
         Canvas.SetTop(rectangle, 0);


         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro zobrazení kalkulačky
         /// /////////////////////////////////

         // Vytvoření tlačítka Kalkulačka
         Button Kalkulacka = new Button
         {
            Background = Brushes.DeepPink,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Kalkulačka",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Kalkulacka.Click += HlavniOkno.KalkulackaButton_Click;

         // Přidání tlačítka Kalkulačka na plátno
         PraveMENU.Children.Add(Kalkulacka);
         Canvas.SetRight(Kalkulacka, 10);
         Canvas.SetTop(Kalkulacka, 100);



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro import dat ze souboru
         /// //////////////////////////////////

         // Vytvoření tlačítka Import
         Button Import = new Button
         {
            Background = Brushes.DarkSalmon,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Import dat",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Import.Click += HlavniOkno.ImportDat_Click;

         // Přidání tlačítka Import na plátno
         PraveMENU.Children.Add(Import);
         Canvas.SetRight(Import, 10);
         Canvas.SetTop(Import, 100 + (RozmeryButton.Vyska + RozmeryButton.Mezera));



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro export dat do souboru
         /// //////////////////////////////////

         // Vytvoření tlačítka Export
         Button Export = new Button
         {
            Background = Brushes.DarkSalmon,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Export dat",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Export.Click += HlavniOkno.ExportDat_Click;

         // Přidání tlačítka Export na plátno
         PraveMENU.Children.Add(Export);
         Canvas.SetRight(Export, 10);
         Canvas.SetTop(Export, 100 + (RozmeryButton.Vyska + RozmeryButton.Mezera) * 2);



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro uložení provedených změn
         /// /////////////////////////////////////

         // Vytvoření tlačítka Ulozit
         Button Ulozit = new Button
         {
            Background = Brushes.OrangeRed,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Uložit",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Ulozit.Click += HlavniOkno.Ulozit_Click;

         // Přidání tlačítka Ulozit na plátno
         PraveMENU.Children.Add(Ulozit);
         Canvas.SetRight(Ulozit, 10);
         Canvas.SetTop(Ulozit, 100 + (RozmeryButton.Vyska + RozmeryButton.Mezera) * 3);



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro možnost změnit nastavení
         /// /////////////////////////////////////

         // Vytvoření tlačítka Nastaveni
         Button Nastaveni = new Button
         {
            Background = Brushes.DarkSalmon,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 16,
            Content = "Nastavení",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Nastaveni.Click += HlavniOkno.Nastaveni_Click;

         // Přidání tlačítka Nastaveni na plátno
         PraveMENU.Children.Add(Nastaveni);
         Canvas.SetRight(Nastaveni, 10);
         Canvas.SetBottom(Nastaveni, 30 + (RozmeryButton.Vyska + RozmeryButton.Mezera) * 2);



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro zobrazení informačního okna
         /// ////////////////////////////////////////

         // Vytvoření tlačítka INFO
         Button Info = new Button
         {
            Background = Brushes.DarkSalmon,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            FontStyle = FontStyles.Italic,
            Content = "Info",
            HorizontalAlignment = HorizontalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Info.Click += HlavniOkno.InformaceButton_Click;

         // Přidání tlačítka Info na plátno
         PraveMENU.Children.Add(Info);
         Canvas.SetRight(Info, 10);
         Canvas.SetBottom(Info, 30 + (RozmeryButton.Vyska + RozmeryButton.Mezera));



         ////////////////////////////////////////////////////////////////////////////////////////////
         /// Tlačítko pro odhlášení
         /// //////////////////////

         // Vytvoření tlačítka pro odhlášení uživatele
         Button Odhlaseni = new Button
         {
            Background = Brushes.DarkRed,
            Width = RozmeryButton.Sirka,
            Height = RozmeryButton.Vyska,
            FontSize = 18,
            Content = "Odhlásit se",
            HorizontalAlignment = HorizontalAlignment.Center
         };

         // Přidání události pro možnost reakce na kliknutí na dané tlačítko
         Odhlaseni.Click += HlavniOkno.Odhlasit_Click;

         // Přidání tlačítka pro odhlášení na plátno
         PraveMENU.Children.Add(Odhlaseni);
         Canvas.SetRight(Odhlaseni, 10);
         Canvas.SetBottom(Odhlaseni, 30);

      }

      
      /// <summary>
      /// Metoda pro vykreslení levého postranního MENU bez ovládacích prvků (naznačení uživatelského MENU).
      /// </summary>
      /// <param name="PraveMENU">Plátno pro vykreslení levého postranního MENU</param>
      public void SkryjLeveMENU(Canvas LeveMENU)
      {
         // Smazání obsahu plátna
         LeveMENU.Children.Clear();

         // Přidání události pro možnost zobrazení MENU
         LeveMENU.MouseMove += HlavniOkno.LeveMENU_MouseMove;

         // Vytvoření obdélníku označující levé MENU
         Rectangle rectangle = new Rectangle
         {
            Fill = Brushes.DarkGreen,
            Width = 30,
            Height = HlavniOkno.Height
         };

         // Přidání obdélníku na plátno
         LeveMENU.Children.Add(rectangle);
         Canvas.SetLeft(rectangle, 0);
         Canvas.SetTop(rectangle, 0);

         // Popisek MENU 
         Label label = new Label
         {
            Content = "M\nE\nN\nU",
            FontSize = 18,
            FontStyle = FontStyles.Oblique,
            FontWeight = FontWeights.SemiBold,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top
         };

         // Přidání popisku na plátno
         LeveMENU.Children.Add(label);
         Canvas.SetLeft(label, 0);
         Canvas.SetTop(label, rectangle.Height / 3);
      }


      /// <summary>
      /// Metoda pro vykreslení pravého postranního MENU bez ovládacích prvků (naznačení uživatelského MENU).
      /// </summary>
      /// <param name="PraveMENU">Plátno pro vykreslení pravého postranního MENU</param>
      public void SkryjPraveMENU(Canvas PraveMENU)
      {
         // Smazání obsahu plátna
         PraveMENU.Children.Clear();

         // Přidání události pro možnost zobrazení MENU
         PraveMENU.MouseMove += HlavniOkno.PraveMENU_MouseMove;

         // Vytvoření obdélníku označující pravé MENU
         Rectangle rectangle = new Rectangle
         {
            Fill = Brushes.DarkGreen,
            Width = 30,
            Height = HlavniOkno.Height
         };

         // Přidání obdélníku na plátno
         PraveMENU.Children.Add(rectangle);
         Canvas.SetRight(rectangle, 0);
         Canvas.SetTop(rectangle, 0);

         // Popisek MENU 
         Label label = new Label
         {
            Content = "M\nE\nN\nU",
            FontSize = 18,
            FontStyle = FontStyles.Oblique,
            FontWeight = FontWeights.SemiBold,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top
         };

         // Přidání popisku na plátno
         PraveMENU.Children.Add(label);
         Canvas.SetRight(label, 0);
         Canvas.SetTop(label, rectangle.Height / 3);
      }


      /// <summary>
      /// Vykreslení informačního okna informujícího o stavu financí v aktuálním měsíci.
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení</param>
      /// <param name="KolekceDat">Kolekce dat, ze kterých se počítají zobrazované údaje</param>
      public void InformacniBlokHlavnihoOkna(Canvas MyCanvas, Brush BarvaPozadi, ObservableCollection<Zaznam> KolekceDat)
      {
         // Inicializace proměnných uchovávající v sobě data k zobrazení v textové podobě
         string AktualniMesic = "";
         string AktualniDen = "";
         string VydajeCelkem = "";
         string PrijmyCelekm = "";
         string Bilance = "";
         string PocetDniDoKonceMesice = "";

         // Rozměry zobrazovaného okna po odečtení orajů
         int sirka = (int)MyCanvas.Width - 2;
         int vyska = (int)MyCanvas.Height - 2;

         // Proměnná pro nastavení výšky textového popisku
         int VyskaTextu = 40;

         // Číselná hodnota dat
         double Bilance_hodnota = 0;
         double Vydaje_hodnota = 0;
         double Prijmy_hodnota = 0;


         // Načtení aktuálního dne a měsíce z aktuálního data
         AktualniDen = hodiny.VratDenVTydnu(DateTime.Now.DayOfWeek);
         AktualniMesic = hodiny.VratMesicTextove(DateTime.Now.Month);

         // Výpočet kolik dní zbývá do konce měsíce
         PocetDniDoKonceMesice = (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day).ToString();

         // Načtení potřebných dat z kolekce
         foreach (Zaznam zaznam in KolekceDat)
         {
            // Přičtení hodnoty k celkové sumě
            if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
               Prijmy_hodnota += zaznam.Hodnota_PrijemVydaj;
            else
               Vydaje_hodnota += zaznam.Hodnota_PrijemVydaj;
         }

         // Výpočet bilance
         Bilance_hodnota = Prijmy_hodnota - Vydaje_hodnota;

         // Pokud je bilance kladná přidá se před ní +, pokud ne tak záporné číso již obsahuje -
         if (Bilance_hodnota > 0)
            Bilance = "+" + Bilance_hodnota.ToString() + " Kč";
         else
            Bilance = Bilance_hodnota.ToString() + " Kč";

         // Převedení získaných hodnot na textové řetězce
         PrijmyCelekm = Prijmy_hodnota.ToString() + " Kč";
         VydajeCelkem = Vydaje_hodnota.ToString() + " Kč";



         // Vytvoření obdélníku pro efekt okrajů
         Rectangle okraje = new Rectangle
         {
            Fill = Brushes.DarkBlue,
            Width = MyCanvas.Width,
            Height = MyCanvas.Height
         };

         // Pozadí informačního bloku
         Rectangle pozadi = new Rectangle
         {
            Fill = BarvaPozadi,
            Width = sirka,
            Height = vyska
         };

         // Oddělovací blok
         Rectangle oddeleni = new Rectangle
         {
            Fill = Brushes.BlueViolet,
            Width = 1,
            Height = vyska - 2 - 2 * VyskaTextu
         };

         // Dělící čára
         Rectangle deliciCara = new Rectangle
         {
            Fill = Brushes.BlueViolet,
            Width = sirka,
            Height = 1
         };


         // Aktuální měsíc
         Label mesic_popisek = new Label
         {
            Content = "Tento měsíc:  ",
            FontSize = 16
         };
         Label mesic = new Label
         {
            Content = AktualniMesic,
            FontSize = 22
         };

         // Aktuální den
         Label den = new Label
         {
            Content = AktualniDen,
            FontSize = 18
         };

         // Výdaje
         Label vydaje_popisek = new Label
         {
            Content = "Výdaje:  ",
            FontSize = 14
         };
         Label vydaje = new Label
         {
            Content = VydajeCelkem,
            FontSize = 18,
            Foreground = Brushes.Red
         };

         // Příjmy
         Label prijmy_popisek = new Label
         {
            Content = "Příjmy:  ",
            FontSize = 14
         };
         Label prijmy = new Label
         {
            Content = PrijmyCelekm,
            FontSize = 18,
            Foreground = Brushes.Green
         };


         // Bilance
         Label Bilance_popisek = new Label
         {
            Content = "Bilance:  ",
            FontSize = 16
         };
         Label bilance = new Label
         {
            Content = Bilance,
            FontSize = 20
         };

         // Rozhodnutí o barvě podle toho jestli je to plus nebo minus
         if (Bilance_hodnota > 0)
            bilance.Foreground = Brushes.Green;

         else if (Bilance_hodnota < 0)
            bilance.Foreground = Brushes.Red;

         else
            bilance.Foreground = Brushes.Black;



         // Informace o počtu dní do konce měsíce
         Label konec = new Label
         {
            Content = "Do konce měsíce zbývá " + PocetDniDoKonceMesice + " dní.",
            FontSize = 14
         };




         // Vykreslení okrajů
         MyCanvas.Children.Add(okraje);
         Canvas.SetLeft(okraje, 0);
         Canvas.SetTop(okraje, 0);

         // Vykreslení pozadí
         MyCanvas.Children.Add(pozadi);
         Canvas.SetLeft(pozadi, 1);
         Canvas.SetTop(pozadi, 1);

         // Vykreslení dělících čar
         MyCanvas.Children.Add(oddeleni);
         MyCanvas.Children.Add(deliciCara);
         Canvas.SetRight(oddeleni, 101);
         Canvas.SetTop(oddeleni, 1);
         Canvas.SetLeft(deliciCara, 1);
         Canvas.SetTop(deliciCara, vyska - 2 - 2 * VyskaTextu);

         // Vykrelsení aktuálního měsíce
         MyCanvas.Children.Add(mesic_popisek);
         MyCanvas.Children.Add(mesic);
         Canvas.SetLeft(mesic_popisek, 1);
         Canvas.SetTop(mesic_popisek, 1);
         Canvas.SetLeft(mesic, 1 + 95);
         Canvas.SetTop(mesic, -2);

         // Vykreslení aktuálního dne
         MyCanvas.Children.Add(den);
         Canvas.SetRight(den, 6);
         Canvas.SetTop(den, 0);

         // Vykreslení výdajů
         MyCanvas.Children.Add(vydaje_popisek);
         MyCanvas.Children.Add(vydaje);
         Canvas.SetLeft(vydaje_popisek, 1);
         Canvas.SetTop(vydaje_popisek, vyska - 1 - 2 * VyskaTextu + 10);
         Canvas.SetLeft(vydaje, 1 + 55);
         Canvas.SetTop(vydaje, vyska - 2 * VyskaTextu + 5);

         // Vykreslení příjmů
         MyCanvas.Children.Add(prijmy_popisek);
         MyCanvas.Children.Add(prijmy);
         Canvas.SetLeft(prijmy_popisek, 1);
         Canvas.SetTop(prijmy_popisek, vyska - 1 - VyskaTextu + 10);
         Canvas.SetLeft(prijmy, 1 + 55);
         Canvas.SetTop(prijmy, vyska - VyskaTextu + 5);

         // Vykreslení Bilance
         MyCanvas.Children.Add(Bilance_popisek);
         MyCanvas.Children.Add(bilance);
         Canvas.SetLeft(Bilance_popisek, sirka / 2);
         Canvas.SetTop(Bilance_popisek, vyska - 1 - 2 * VyskaTextu + 10);
         Canvas.SetLeft(bilance, (sirka / 2) + 60);
         Canvas.SetTop(bilance, vyska - 2 * VyskaTextu + 5);

         // Vykrelení věty o počtu dní do konce měsíce
         MyCanvas.Children.Add(konec);
         Canvas.SetRight(konec, 6);
         Canvas.SetBottom(konec, 3);
      }


      /// <summary>
      /// Metoda pro vykreslení ukazatele síly hesla.
      /// Na základě celkového počtu podmínek a počtu splněných podmínek graficky zobrazí sílu zadaného hesla.
      /// </summary>
      /// <param name="MyCanvas">Vykreslovací plátno z okenního formuláře</param>
      /// <param name="PocetSplnenychPodminek">Počet splněných podmínek (počet zelených bloků)</param>
      /// <param name="CelkovyPocetPodminek">Celkový počet podmínek (počet všech bloků)</param>
      /// <param name="Sirka">Celková šířka ukazatele</param>
      /// <param name="Vyska">Celkový výška ukazatele</param>
      public void VykresleniUkazateleHesla(Canvas MyCanvas, int PocetSplnenychPodminek, int CelkovyPocetPodminek, int Sirka, int Vyska)
      {
         Brush BarvaOkraje = Brushes.Black;     // Nastavení barvy okraje ukazatele
         int VyskaUkazatele = Vyska;            // Nastavení výšky ukazatele
         int SirkaUkazatele = Sirka;            // Nastavení šířky ukazatele


         // Vytvoření levého okraje ukazatele a nastavení počátečních souřadnic včetně nastavení barvy
         Rectangle LevyOkraj = new Rectangle
         {
            Height = VyskaUkazatele,
            Width = 1
         };
         LevyOkraj.Fill = BarvaOkraje;       // Nastavení barvy okraje
         MyCanvas.Children.Add(LevyOkraj);   // Přidání levého okraje do canvasu
         Canvas.SetLeft(LevyOkraj, 0);       // Nastavení počátečních souřadnic
         Canvas.SetTop(LevyOkraj, 0);        // Nastavení počátečních souřadnic


         // Cyklus pro vytvoření okrajů následně vykreslovaných obdélníků
         for (int i = 0; i < CelkovyPocetPodminek; i++)
         {
            // Vytvoření nového obdélníku
            Rectangle OkrajovyObdelnik = new Rectangle
            {
               Height = VyskaUkazatele,
               Width = (SirkaUkazatele - 1) / CelkovyPocetPodminek
            };
            OkrajovyObdelnik.Fill = BarvaOkraje;      // Nastavení barvy dle arvy okraje
            MyCanvas.Children.Add(OkrajovyObdelnik);  // Přidání okrajového obdélníku do canvasu

            // Nastavení souřadnic pro vykreslení okrajových obdélníků
            Canvas.SetLeft(OkrajovyObdelnik, i * OkrajovyObdelnik.Width + 1);
            Canvas.SetTop(OkrajovyObdelnik, 0);
         }


         // Cyklus pro vytvoření požadovaného počtu obdélníku zarovnaných do canvasu vedle sebe s mezerou 1p
         for (int i = 0; i < CelkovyPocetPodminek; i++)
         {
            // Vytvoření nového obdélníku
            Rectangle obdelnik = new Rectangle
            {
               Height = VyskaUkazatele - 2,                                                  // Nastavení výšky zobrazovaných bloků 
               Width = (SirkaUkazatele - 1 - CelkovyPocetPodminek) / CelkovyPocetPodminek    // Určení šířky 1 obdélníku podle celkové šířky ukazatele 
            };

            // Nastavení barvy obdélníku (počet zelených obdélníku je dán počtem splněných podmínek)
            obdelnik.Fill = PocetSplnenychPodminek > i ? Brushes.Green : Brushes.Red;

            // Přidání vytvořeného obdélníku mezi potomky vykreslované na canvas
            MyCanvas.Children.Add(obdelnik);

            // Nastavení počátečních souřadnic obdélníku pro vykreslení na požadované místo
            Canvas.SetLeft(obdelnik, i * (obdelnik.Width + 1) + 1);
            Canvas.SetTop(obdelnik, 1);  // Posunutí o 1 p dolů -> horní okraj
         }
      }

   }
}
