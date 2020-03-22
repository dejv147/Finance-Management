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
   /// Třída zpracovává kolekci položek a vytváří grafickou stránku pro vykreslení na plátno.
   /// </summary>
   public class GrafickePolozky
   {
      /// <summary>
      /// Instance přidávacího okna pro možnost předávat data
      /// </summary>
      private PridatPolozkyWindow OknoPolozek;

      /// <summary>
      /// Instance upravovacího okna pro možnost zobrazení.
      /// </summary>
      private UpravitZaznamWindow OknoZaznamu;

      /// <summary>
      /// Interní kolekce pro zpracování dat.
      /// </summary>
      public ObservableCollection<Polozka> SeznamPolozek { get; private set; }

      /// <summary>
      /// Kolekce vybraných dat vykreslovaných na 1 stránku seznamu.
      /// </summary>
      public ObservableCollection<Polozka> PolozkyNaJedneStrance { get; private set; }

      /// <summary>
      /// Konstanta udávající počet vykreslených položek, které se vejdou na 1 stránku.
      /// </summary>
      public const int MaxPolozekNaStranku = 5;

      /// <summary>
      /// Grafický prvek zobrazující vykreslenou celou stránku seznamu, tedy určitý počet položek na 1 stránce.
      /// Tento prvek je brán v přidávacím okně a je vykreslován.
      /// </summary>
      public StackPanel StrankaPolozek { get; private set; }

      /// <summary>
      /// Grafický prvek pro vykreslení informační bubliny pro zvolenou položku.
      /// </summary>
      public StackPanel InfoPolozka { get; private set; }

      /// <summary>
      /// Položka ze seznamu, která byla uživatelem vybrána
      /// </summary>
      public Polozka VybranaPolozka { get; private set; }

      /// <summary>
      /// Pomocná proměnná  pro uchování minulového vybraného bloku ze seznamu pro možnost zrušit jeho označení při výběru jiného bloku.
      /// </summary>
      private StackPanel OznacenyBlok;

      /// <summary>
      /// Interní proměnná pro identifikaci vykreslované stránky
      /// </summary>
      private int CisloStranky;

      /// <summary>
      /// Interní proměnná pro uložení celkového počtu stran v závislosti na počtu dat v kolekci
      /// </summary>
      private int MaximalniPocetStran;

      /// <summary>
      /// Příznakový bit informující zda je vykresleno informační okno pro vybranou položku.
      /// </summary>
      private byte InfoVykresleno;

      /// <summary>
      /// Příznakový bit pro rozlišení do jakého okna se seznam položek bude vykreslovat.
      /// </summary>
      private byte PridavaciOkno;


      /// <summary>
      /// Konstruktor třídy pro možnost zobrazit grafický seznam položek v okně pro přidání položek.
      /// </summary>
      /// <param name="SeznamPolozek">Kolekce položek</param>
      /// <param name="OknoPridatPolozky">Instance přidávácího okna</param>
      public GrafickePolozky(ObservableCollection<Polozka> SeznamPolozek, PridatPolozkyWindow OknoPridatPolozky)
      {
         // Úvodní nastavení při otevření okna
         UvodniNastaveni();

         // Určení okna pro přidávání
         PridavaciOkno = 1;

         // Přiřazení předaných dat do interních proměnných
         OknoPolozek = OknoPridatPolozky;
         this.SeznamPolozek = SeznamPolozek; 
      }


      /// <summary>
      /// Konstruktor třídy pro možnost zobrazit grafický seznam položek v okně ro úpravu záznamu.
      /// </summary>
      /// <param name="SeznamPolozek">Kolekce položek</param>
      /// <param name="OknoPridatPolozky">Instance okna pro zobrazení</param>
      public GrafickePolozky(ObservableCollection<Polozka> SeznamPolozek, UpravitZaznamWindow OknoUpravitZazanam)
      {
         // Úvodní nastavení při otevření okna
         UvodniNastaveni();

         // Určení okna pro pouhe zobrazení položek (okno upravit záznam)
         PridavaciOkno = 0;

         // Přiřazení předaných dat do interních proměnných
         this.OknoZaznamu = OknoUpravitZazanam;
         this.SeznamPolozek = SeznamPolozek;
      }




      /// <summary>
      /// Metoda pro aktualizaci dat v interní kolekci pro zpracování
      /// </summary>
      /// <param name="SeznamPolozek">Seznam položek ke zpracování</param>
      public void ObnovKolekciPolozek(ObservableCollection<Polozka> SeznamPolozek)
      {
         this.SeznamPolozek = SeznamPolozek;
         MaximalniPocetStran = (int)Math.Ceiling((double)this.SeznamPolozek.Count / (double)MaxPolozekNaStranku);

         // Aktualizace grafických dat
         AktualizujVykreslenouStranku();
      }

      /// <summary>
      /// Vykreslení seznamu položek při úvodním nastavení
      /// </summary>
      public void UvodniNastaveni()
      {
         // Úvodní inicializace interních kolekcí a proměných
         CisloStranky = 0;
         InfoVykresleno = 0;
         this.StrankaPolozek = new StackPanel();
         this.SeznamPolozek = new ObservableCollection<Polozka>();
         PolozkyNaJedneStrance = new ObservableCollection<Polozka>();
         OznacenyBlok = new StackPanel();
         InfoPolozka = new StackPanel();

         // Určení počtu stran pro výpis dat z kolekce
         MaximalniPocetStran = (int)Math.Ceiling((double)this.SeznamPolozek.Count / (double)MaxPolozekNaStranku);

         // Úvodní vykreslení seznamu
         VyberPolozkyNaStranku();                              // Vybrání dat pro první stránku seznamu
         VytvorGrafickouStrankuPolozek();                      // Vytvoření kolekce graficky reprezentovaných dat
      }

      /// <summary>
      /// Opětovné vykreslení seznamu položek při změně dat.
      /// </summary>
      public void AktualizujVykreslenouStranku()
      {
         // Výběr určitého množství položek pro vykreslení
         VyberPolozkyNaStranku();

         // Kontrola zda jsou na zadané stránce alespoň nějaké položky k vykreslení (zabezpečení změny stránky v případě vymazání všech položek uživatelem na zobrazované stránce)
         if (PolozkyNaJedneStrance.Count == 0)
         {
            this.CisloStranky--;
            VyberPolozkyNaStranku();
         }

         // Vytvoření grafické reprezentace vybraných dat
         VytvorGrafickouStrankuPolozek();
      }

      /// <summary>
      /// Metoda pro vytvoření interní kolekce obsahující pouze položky vykreslované na konkrétní stránku
      /// </summary>
      private void VyberPolozkyNaStranku()
      {
         // Určení indexu položky pro výběr na základě počtu již vykreslených stran
         int StartIndex = CisloStranky * MaxPolozekNaStranku;

         // Smazání kolekce pro možnost přidání nových dat
         PolozkyNaJedneStrance.Clear();

         // Pokud v kolekci je více položek než se vejde na 1 stránku, vybere se pouze maximální počet na stránku
         if ((StartIndex + MaxPolozekNaStranku) <= SeznamPolozek.Count)
         {
            // Postupné přidání určitého počtu položek do kolkece pro vykreslení 1 strany
            for (int index = StartIndex; index < (StartIndex + MaxPolozekNaStranku); index++)
            {
               PolozkyNaJedneStrance.Add(SeznamPolozek[index]);
            }
         }
         // Pokud v kolekci zbýva jen několik položek pro vykreslení, vybere se daný počet na poslední stránku
         else
         {
            // Postupné přidání určitého počtu položek do kolkece pro vykreslení 1 strany
            for (int index = StartIndex; index < SeznamPolozek.Count; index++)
            {
               PolozkyNaJedneStrance.Add(SeznamPolozek[index]);
            }
         }
      }

      /// <summary>
      /// Metoda pro vykreslení informační bubliny předané položky.
      /// </summary>
      /// <param name="polozka">Položka pro vykreslení</param>
      private void VykresliInfoBublinu(Polozka polozka)
      {
         //Smazání předchozího obsahu
         this.InfoPolozka.Children.Clear();

         // Vytvoření bloku reprezentující informační okno 1 položky
         StackPanel GrafPolozkaInfo = new StackPanel
         {
            Orientation = Orientation.Vertical,
            Background = Brushes.Ivory,
            Width = 190,
            Height = 100
         };

         // Kategorie položky
         Label kategorie = new Label
         {
            Content = polozka.KategoriePolozky.ToString(),
            FontSize = 17
         };

         // Podtržení 
         Rectangle cara = new Rectangle
         {
            Width = 190,
            Height = 1,
            Fill = Brushes.Red
         };

         // Popis položky
         Label popis = new Label
         {
            Content = polozka.Popis,
            FontSize = 15
         };

         // Přidání prvku do informačního bloku položky
         GrafPolozkaInfo.Children.Add(kategorie);
         GrafPolozkaInfo.Children.Add(cara);
         GrafPolozkaInfo.Children.Add(popis);

         // Uložení vytvořeného bloku do interní proměnné
         this.InfoPolozka.Children.Add(GrafPolozkaInfo);
      }
      
      /// <summary>
      /// Vytvoření grafické reprezentace položek na 1 stránce a seskupení těchto položek do interní kolekce.
      /// </summary>
      private void VytvorGrafickouStrankuPolozek()
      {
         // Smazání obsahu stránky za účelem vykreslení nových dat
         this.StrankaPolozek.Children.Clear();

         // Vytvoření StackPanel pro seskupení grafických prvků na 1 stránce
         StackPanel Stranka = new StackPanel
         {
            Orientation = Orientation.Vertical
         };

         // Postupné vytvoření grafické reprezentace položky a přidání do kolkece grafických položek
         foreach (Polozka polozka in PolozkyNaJedneStrance)
         {
            // Vytvoření bloku reprezentující 1 položku na stránce
            StackPanel GrafPolozka = new StackPanel
            {
               Orientation = Orientation.Vertical
            };

            // Název položky
            Label nazev = new Label
            {
               Content = polozka.Nazev + ":",
               FontSize = 17
            };

            // Hodnota položky
            Label cena = new Label
            {
               Content = polozka.Cena.ToString() + " Kč",
               FontSize = 18
            };

            // Oddělovací čára
            Rectangle deliciCara = new Rectangle
            {
               Width = 200,
               Height = 2,
               Fill = Brushes.DarkBlue
            };

            // Přidání prvků do bloku položky
            GrafPolozka.Children.Add(nazev);
            GrafPolozka.Children.Add(cena);
            GrafPolozka.Children.Add(deliciCara);

            // Uložení indexu položky v seznamu pro následnou identifikaci konrétní položky
            GrafPolozka.Name ="obr" +  PolozkyNaJedneStrance.IndexOf(polozka).ToString();

            // Přidání událostí pro grafický blok určený pro vykreslení stránky položek
            GrafPolozka.MouseDown += Polozka_MouseDown;
            GrafPolozka.MouseMove += Polozka_MouseMove;
            GrafPolozka.MouseLeave += Polozka_MouseLeave;

            // Přidání bloku na stránku
            Stranka.Children.Add(GrafPolozka);
         }



         // Doplnění stránky práznými bloky pro zajištění stálé velikosti stránky z důvodu pohodlnějšího ovládání kolečkem myši (funguje pouze pokud je ukazatel myši na daném prvku)
         
         // Počet prázdných bloků potřebných pro doplnění stránky
         int PocetPrazdnychBloku = MaxPolozekNaStranku - PolozkyNaJedneStrance.Count;

         // Pomocná proměnná pro uchování výšky vytvořeného bloku
         int VyskaPrazdnehoBloku = (int)Math.Floor(360.0 / MaxPolozekNaStranku);

         // Přidání určitého počtu prázdných bloků pro dpolnění stránky na celkovou velikost
         for (int i = 0; i < PocetPrazdnychBloku; i++)
         {
            Rectangle prazdnyBlok = new Rectangle
            {
               Fill = Brushes.LightBlue,
               Height = VyskaPrazdnehoBloku,
               Width = 200
            };
            Stranka.Children.Add(prazdnyBlok);
         }


         // Vytvoření popisku pro označení aktuální stránky
         Label OznaceniStranky = new Label
         {
            FontSize = 12,
            Content = "Strana " + (CisloStranky + 1).ToString() + " z " + MaximalniPocetStran.ToString(),
            Foreground = Brushes.Red
         };

         // Přidání popisu do bloku stránky
         Stranka.Children.Add(OznaceniStranky);

         // Uložení indexu stránky pro identifikaci dat a orientaci v celkovém seznamu
         Stranka.Name = "str" + CisloStranky.ToString();

         // Přidání události pro stránku graficky vykreslených dat
         Stranka.MouseWheel += Stranka_MouseWheel;

         // Uložení grafické reprezentace stránky do interní proměnné
         this.StrankaPolozek.Children.Add(Stranka);
      }

      

      /// <summary>
      /// Obsluha události při odjetí myši z konkrétního bloku reprezentující 1 položku na stránce
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Polozka_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 1)
         {
            // Pokud jsou položky vykresleny v okně pro úpravu záznamu, nevykreslují informační bublinu
            if (PridavaciOkno == 0)
            {
               return;
            }

            // Smazání informační bubliny v přidávacím okně
            OknoPolozek.SmazInfoPolozky();
            
            // Smazání interní proměnné uchovávající informační bublinu
            InfoPolozka.Children.Clear();

            // Nastavení příznakové proměnné pro možnost dalšího vykreslování
            InfoVykresleno = 0;
         }
      }

      /// <summary>
      /// Obsluha události při najetí myši na konkrétní blok reprezentující 1 položku na stránce
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Polozka_MouseMove(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 0)
         {
            // Převedení zvoleného objektu zpět na StackPanel
            StackPanel blok = sender as StackPanel;

            // Pokud jsou položky vykresleny v okně pro úpravu záznamu, nevykreslují informační bublinu
            if (PridavaciOkno == 0)
            {
               return;
            }

            // Odstranění prefixu "obr" z názvu bloku
            string IndexPolozky = blok.Name.Substring(3);

            // Identifikace položky na základě indexu objektu (Zjištění o jakou položku se jedná) a vytvoření informační buliny dané položky
            VykresliInfoBublinu(PolozkyNaJedneStrance[int.Parse(IndexPolozky)]);

            // Vykreslení informační bubliny v přidávacím okně
            OknoPolozek.ZobrazInfoPolozky();

            // Nastavení příznakové proměnné pro zamezení opětovného vykreslování
            InfoVykresleno = 1;
         }
      }

      /// <summary>
      /// Obsluha události při kliknutí na konkrétní blok reprezentující 1 položku na stránce
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Polozka_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Pokud jsou položky vykresleny v okně pro úpravu záznamu, událost pro kliknutí se neobslouží
         if (PridavaciOkno == 0)
            return;
       
         // Převedení zvoleného objektu zpět na StackPanel
         StackPanel blok = sender as StackPanel;

         // Barevné vyznačení vybraného objektu
         blok.Background = Brushes.Orange;

         // Zrušení barevného vyznačení předchozího vybraného objektu
         OznacenyBlok.Background = Brushes.LightBlue;

         // Uložení nově označeného objektu do pomocné proměnné pro možnost následného zrušení jeho označení při označení jiného objektu
         OznacenyBlok = blok;

         // Odstranění prefixu "obr" z názvu bloku
         string IndexPolozky = blok.Name.Substring(3);

         // Identifikace položky na základě indexu objektu -> Zjištění o jakou položku se jedná (přiřazení do VybranaPolozka)
         VybranaPolozka = PolozkyNaJedneStrance[int.Parse(IndexPolozky)];

         // Předání vybrané položky do přidávacího okna pro možnost pracovat s vybranou položkou
         OknoPolozek.VyberPolozku(VybranaPolozka);

         // V případě dvojkliku se vyvolá okno pro úpravu vybrané položky
         if (e.ClickCount > 1)
         {
            OknoPolozek.UpravitPolozku();
         }
      }

      /// <summary>
      /// Událost vyvolaná při pohybu kolečka myši pro celou stránku
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Stranka_MouseWheel(object sender, MouseWheelEventArgs e)
      {
         // Převedení vybraného objektu zpět na StackPanel
         StackPanel stranka = sender as StackPanel;

         // Odstranění prefixu "str" z názvu bloku
         string IndexStrany = stranka.Name.Substring(3);

         // Identifikace aktuálně vykreslené strany seznamu na základě čísla uloženého v názvu StackPanelu
         int AktualniStranka = int.Parse(IndexStrany);

         // Pokud je vykreslena první stránka seznamu a uživatel pohne kolečkem myši nahoru, nic se nestane
         if (AktualniStranka == 0 && (e.Delta > 0)) 
         {
            return;
         }

         // Pokud je vykreslena první stránka seznamu a uživatel pohne kolečkem myši dolů, 
         // změní se číslo vykreslované stránky a aktualizuje se vykreslení (vykreslí se nová stránka)
         else if (AktualniStranka == 0 && (e.Delta <= 0))
         {
            CisloStranky++;
            AktualizujVykreslenouStranku();
            return;
         }

         // Pokud je vykreslena poslední stránka seznamu a uživatel pohne kolečkem myši dolů, nic se nestane
         if ((AktualniStranka + 1) == MaximalniPocetStran && (e.Delta <= 0))
         {
            return;
         }

         // Pokud je vykreslena poslední stránka seznamu a uživatel pohne kolečkem myši nahorů, 
         // změní se číslo vykreslované stránky a aktualizuje se vykreslení (vykreslí se nová stránka)
         else if ((AktualniStranka + 1) == MaximalniPocetStran && (e.Delta > 0)) 
         {
            CisloStranky--;
            AktualizujVykreslenouStranku();
            return;
         }

         // Pokud uživatel pohnul kolečkem myši nahorů vykreslí se nová stránka v reakci na změnu čísla stránky
         if (e.Delta > 0)
         {
            CisloStranky--;
            AktualizujVykreslenouStranku();
            return;
         }

         // Pokud uživatel pohnul kolečkem myši dolů vykreslí se nová stránka v reakci na změnu čísla stránky
         else if (e.Delta <= 0) 
         {
            CisloStranky++;
            AktualizujVykreslenouStranku();
            return;
         }
      }

   }
}
