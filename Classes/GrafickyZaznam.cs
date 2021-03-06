﻿using System;
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
   /// Třída zpracovává kolekci záznamů a vytváří grafickou stránku pro vykreslení na plátno.
   /// </summary>
   public class GrafickyZaznam
   {
      /// <summary>
      /// Kolekce všech záznamů, ve které se vyhledávají potřebná data.
      /// </summary>
      public ObservableCollection<Zaznam> SeznamZaznamu { get; private set; }

      /// <summary>
      /// Instance hlavního okna pro možnost komunikace při vykreslování v hlavním oně
      /// </summary>
      private MainWindow HlavniOkno;

      /// <summary>
      /// Instance třídy pro vyhledávání konkrétních dat
      /// </summary>
      private Vyhledavani vyhledavani;

      /// <summary>
      /// Kolekce vybraných dat vykreslovaných na 1 stránku seznamu.
      /// </summary>
      public ObservableCollection<Zaznam> ZaznamyNaJedneStrance { get; private set; }

      /// <summary>
      /// Konstanta udávající počet vykreslených položek, které se vejdou na 1 stránku.
      /// </summary>
      public const int MaxZaznamuNaStranku = 7;

      /// <summary>
      /// Grafický prvek zobrazující vykreslenou celou stránku seznamu, tedy určitý počet položek na 1 stránce.
      /// Tento prvek je brán v přidávacím okně a je vykreslován.
      /// </summary>
      public StackPanel StrankaZaznamu { get; private set; }

      /// <summary>
      /// Plátno pro vykreslení informační bubliny vybraného záznamu
      /// Plátno je předáváno v konstruktoru, tedy instance uchovává plátno v okně, ve kterém byla instance třídy vytvořena.
      /// </summary>
      public Canvas InfoCanvas { get; private set; }

      /// <summary>
      /// Položka ze seznamu, která byla uživatelem vybrána
      /// </summary>
      public Zaznam VybranyZaznam { get; private set; }

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
      /// Příznakový bit informující zda je vykresleno informační okno pro vybraný záznam.
      /// </summary>
      private byte InfoVykresleno;



      /// <summary>
      /// Konstruktor třídy pro grafické vykreslování záznamů v hlavním okně aplikace.
      /// </summary>
      /// <param name="HlavniOkno">Instance hlavního okna aplikace</param>
      /// <param name="InfoBublina">Plátno pro vykreslení informační bubliny záznamu</param>
      public GrafickyZaznam(MainWindow HlavniOkno, Canvas InfoBublina)
      {
         // Úvodní inicializace interních kolekcí a proměných
         CisloStranky = 0;
         InfoVykresleno = 0;
         SeznamZaznamu = new ObservableCollection<Zaznam>();
         ZaznamyNaJedneStrance = new ObservableCollection<Zaznam>();
         StrankaZaznamu = new StackPanel();
         OznacenyBlok = new StackPanel();
         vyhledavani = new Vyhledavani();
         InfoCanvas = InfoBublina;
         this.HlavniOkno = HlavniOkno;

         // Určení počtu stran pro výpis dat z kolekce
         MaximalniPocetStran = (int)Math.Ceiling((double)SeznamZaznamu.Count / (double)MaxZaznamuNaStranku);
      }



      /// <summary>
      /// Aktualizace dat v interní kolekci a obnova vykreslených dat
      /// </summary>
      /// <param name="Zaznamy">Kolekce záznamů ke zpracování</param>
      public void ObnovKolekciZaznamu(ObservableCollection<Zaznam> Zaznamy)
      {
         SeznamZaznamu = Zaznamy;
         MaximalniPocetStran = (int)Math.Ceiling((double)Zaznamy.Count / (double)MaxZaznamuNaStranku);
         SeznamZaznamu = vyhledavani.SeradZaznamy(SeznamZaznamu);
         AktualizujVykreslenouStranku();
      }

      /// <summary>
      /// Opětovné vykreslení kolekce záznamů při změně dat.
      /// </summary>
      public void AktualizujVykreslenouStranku()
      {
         // Zrušení označení vybraného záznamu
         if (HlavniOkno != null) 
            HlavniOkno.ZrusOznaceniZaznamu();

         // Výběr určitého počtu záznamů pro vykreslení
         VyberZaznamyNaStranku();

         // Kontrola zda jsou na zadané stránce alespoň nějaké záznamy k vykreslení (zabezpečení změny stránky v případě vymazání všech záznamů uživatelem na zobrazované stránce)
         if (ZaznamyNaJedneStrance.Count == 0 && CisloStranky > 0) 
         {
            this.CisloStranky--;
            VyberZaznamyNaStranku();
         }
         
         // Vytvoření grafické reprezentace vybraných dat
         VytvorGrafickouStrankuZaznamu();
      }

      /// <summary>
      /// Metoda pro vytvoření interní kolekce obsahující pouze záznamy vykreslované na konkrétní stránku.
      /// </summary>
      private void VyberZaznamyNaStranku()
      {
         // Určení indexu záznamu pro výběr na základě počtu již vykreslených stran
         int StartIndex = CisloStranky * MaxZaznamuNaStranku;

         // Smazání kolekce pro možnost přidání nových dat
         ZaznamyNaJedneStrance.Clear();

         // Pokud v kolekci je více položek než se vejde na 1 stránku, vybere se pouze maximální počet na stránku
         if ((StartIndex + MaxZaznamuNaStranku) <= SeznamZaznamu.Count)
         {
            // Postupné přidání určitého počtu položek do kolkece pro vykreslení 1 strany
            for (int index = StartIndex; index < (StartIndex + MaxZaznamuNaStranku); index++)
            {
               ZaznamyNaJedneStrance.Add(SeznamZaznamu[index]);
            }
         }

         // Pokud v kolekci zbýva jen několik záznamů pro vykreslení, vybere se daný počet na poslední stránku
         else
         {
            // Postupné přidání určitého počtu položek do kolkece pro vykreslení 1 strany
            for (int index = StartIndex; index < SeznamZaznamu.Count; index++)
            {
               ZaznamyNaJedneStrance.Add(SeznamZaznamu[index]);
            }
         }
      }

      /// <summary>
      /// Vytvoření grafické reprezentace záznamů na 1 stránce a seskupení těchto položek do interní kolekce
      /// </summary>
      private void VytvorGrafickouStrankuZaznamu()
      {
         // Smazání obsahu stránky za účelem vykreslení nových dat
         this.StrankaZaznamu.Children.Clear();

         // Vytvoření StackPanel pro seskupení grafických prvků na 1 stránce
         StackPanel Stranka = new StackPanel
         {
            Orientation = Orientation.Vertical
         };

         // Postupné vytvoření grafické reprezentace záznamu a přidání do kolkece grafických záznamů
         foreach(Zaznam zaznam in ZaznamyNaJedneStrance)
         {
            // Převedení kategorie (jedná se o příjem nebo výdaj) na textový řetězec s diakritikou
            string PrijemVydaj = "";
            if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
               PrijemVydaj = "Příjem";
            else
               PrijemVydaj = "Výdaj";


            // Vytvoření bloku reprezentující 1 záznam na stránce
            StackPanel GrafZaznam = new StackPanel
            {
               Orientation = Orientation.Vertical
            };

            // Název položky
            Label nazev = new Label
            {
               Content = zaznam.Datum.Date.ToString("dd.MM.yyyy") + "  -> " + zaznam.Nazev,
               FontSize = 16
            };

            // Hodnota záznamu
            Label cena = new Label
            {
               Content = PrijemVydaj + ": " + zaznam.Hodnota_PrijemVydaj.ToString() + " Kč",
               FontSize = 16
            };

            // Oddělovací čára
            Rectangle deliciCara = new Rectangle
            {
               Width = 300,
               Height = 2,
               Fill = Brushes.DarkBlue
            };

            // Přidání prvků do bloku záznamu
            GrafZaznam.Children.Add(nazev);
            GrafZaznam.Children.Add(cena);
            GrafZaznam.Children.Add(deliciCara);

            // Uložení indexu záznamu v seznamu pro následnou identifikaci konkrétního záznamu
            GrafZaznam.Name = "obr" + ZaznamyNaJedneStrance.IndexOf(zaznam).ToString();

            // Přidání událostí pro grafický blok určený pro vykreslení stránky záznamů
            GrafZaznam.MouseDown += GrafZaznam_MouseDown;
            GrafZaznam.MouseMove += GrafZaznam_MouseMove;
            GrafZaznam.MouseLeave += GrafZaznam_MouseLeave;

            // Přidání bloku na stránku
            Stranka.Children.Add(GrafZaznam);
         }



         // Doplnění stránky práznými bloky pro zajištění stálé velikosti stránky z důvodu pohodlnějšího ovládání kolečkem myši (funguje pouze pokud je ukazatel myši na daném prvku)

         // Počet prázdných bloků potřebných pro doplnění stránky
         int PocetPrazdnychBloku = MaxZaznamuNaStranku - ZaznamyNaJedneStrance.Count;

         // Pomocná proměnná pro uchování výšky vytvořeného bloku
         int VyskaPrazdnehoBloku = (int)Math.Floor(450.0 / MaxZaznamuNaStranku);

         // Přidání určitého počtu prázdných bloků pro dpolnění stránky na celkovou velikost
         for (int i = 0; i < PocetPrazdnychBloku; i++)
         {
            Rectangle prazdnyBlok = new Rectangle
            {
               Height = VyskaPrazdnehoBloku,
               Width = 300
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
         this.StrankaZaznamu.Children.Add(Stranka);
      }

      /// <summary>
      /// Metoda pro vykreslení informační bubliny předaného záznamu na plátno uložené v interní proměnné třídy.
      /// Plátno pro vykreslení je předáváno v konstruktoru této třídy.
      /// </summary>
      /// <param name="zaznam">Záznam pro vykreslení</param>
      private void VykresliInfoBublinu(Zaznam zaznam)
      {
         // Smazání předchozího obsahu
         this.InfoCanvas.Children.Clear();

         // Vytvoření textového řetězce obsahujícího názvy všech položek daného záznamu
         string SeznamPolozek = "";
         foreach (Polozka polozka in zaznam.SeznamPolozek)
         {
            SeznamPolozek += polozka.Nazev + "; ";
         }
         // Odstranění posledních 2 znaků (; ) z textového řetězce
         if (SeznamPolozek.Length > 0)
            SeznamPolozek = SeznamPolozek.Remove(SeznamPolozek.Length - 2);

         // Vytvoření obdélníku na pozadí pro vytvoření efektu okrajů
         Rectangle okraje = new Rectangle
         {
            Fill = Brushes.DarkBlue,
            Width = InfoCanvas.Width,
            Height = InfoCanvas.Height
         };

         // Vytvoření pozadí informační bubliny
         Rectangle pozadi = new Rectangle
         {
            Fill = Brushes.Ivory,
            Width = InfoCanvas.Width - 2,
            Height = InfoCanvas.Height - 2
         };

         // Podtržení
         Rectangle deliciCara = new Rectangle
         {
            Width = InfoCanvas.Width,
            Height = 1,
            Fill = Brushes.DarkBlue
         };

         // Podtržení
         Rectangle deliciCara2 = new Rectangle
         {
            Width = InfoCanvas.Width,
            Height = 1,
            Fill = Brushes.DarkBlue
         };

         // Kategorie záznamu
         Label kategorie = new Label
         {
            Content = "Kategorie:  " + zaznam.Kategorie.ToString(),
            FontSize = 14,
            HorizontalContentAlignment = HorizontalAlignment.Left
         };

         // Oddělení popisků
         Rectangle oddeleni = new Rectangle
         {
            Width = 1,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = Brushes.DarkBlue
         };

         // Datum vytvoření záznamu
         Label datum = new Label
         {
            Content = "Přidáno dne: " + zaznam.DatumZapisu.Date.ToString("dd.MM.yyyy"),
            FontSize = 14
         };

         // Poznámka záznamu
         TextBox poznamka = new TextBox
         {
            Text = zaznam.Poznamka,
            FontSize = 12,
            Background = Brushes.Ivory,
            TextWrapping = TextWrapping.Wrap,
            Width = InfoCanvas.Width - 2,
            Height = Math.Ceiling((InfoCanvas.Height - 34) / 2)
         };

         // Vypsání názvů položek
         TextBox polozky = new TextBox
         {
            Text = SeznamPolozek,
            FontSize = 12,
            Background = Brushes.Ivory,
            TextWrapping = TextWrapping.Wrap,
            Width = InfoCanvas.Width - 2,
            Height = Math.Ceiling((InfoCanvas.Height - 34) / 2)
         };

         // Přidání okrajů na plátno
         InfoCanvas.Children.Add(okraje);
         Canvas.SetLeft(okraje, 0);
         Canvas.SetTop(okraje, 0);

         // Přidání pozadí bubliny na plátno
         InfoCanvas.Children.Add(pozadi);
         Canvas.SetLeft(pozadi, 1);
         Canvas.SetTop(pozadi, 1);

         // Vypsání kategorie na plátno
         InfoCanvas.Children.Add(kategorie);
         Canvas.SetLeft(kategorie, 1);
         Canvas.SetTop(kategorie, 1);

         // Vypsání data úpravy na plátno
         InfoCanvas.Children.Add(datum);
         Canvas.SetRight(datum, 3);
         Canvas.SetTop(datum, 1);

         // Přidání oddělovací přepážky na plátno
         InfoCanvas.Children.Add(oddeleni);
         Canvas.SetLeft(oddeleni, InfoCanvas.Width / 2);
         Canvas.SetTop(oddeleni, 1);

         // Přidání dělící čáry na plátno
         InfoCanvas.Children.Add(deliciCara);
         Canvas.SetLeft(deliciCara, 0);
         Canvas.SetTop(deliciCara, 30);

         // Vypsání textu poznámky
         InfoCanvas.Children.Add(poznamka);
         Canvas.SetLeft(poznamka, 1);
         Canvas.SetTop(poznamka, 31);

         // Přidání dělící čáry na plátno
         InfoCanvas.Children.Add(deliciCara2);
         Canvas.SetLeft(deliciCara2, 0);
         Canvas.SetTop(deliciCara2, (31 + poznamka.Height));

         // Vypsání názvů položek
         InfoCanvas.Children.Add(polozky);
         Canvas.SetLeft(polozky, 1);
         Canvas.SetTop(polozky, (32 + poznamka.Height));
      }

      /// <summary>
      /// Předání vybraného záznamu ze zobrazené stránky do hlavního okna aplikace pro možnost následného zpracování. 
      /// </summary>
      /// <param name="e">Vyvolaná událost</param>
      private void PredejZaznamOknu(MouseButtonEventArgs e)
      {
         // Pokud není předána instance hlavního okna, metoda se ukončí
         if (HlavniOkno == null)
            return;

         /////////////////////////////////////////////////////////////////////////
         // Předání vybraného záznamu do hlavního okna
         HlavniOkno.VyberZaznam(VybranyZaznam);

         // V případě dvojkliku se vyvolá okno pro úpravu vybraného záznamu
         if (e.ClickCount > 1)
         {
            HlavniOkno.UpravitZaznam();
         }
         /////////////////////////////////////////////////////////////////////////
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

      /// <summary>
      /// Obsluha události při odjetí kurzoru myši z daného bloku v seznamu.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná udáost</param>
      private void GrafZaznam_MouseLeave(object sender, MouseEventArgs e)
      {
         // Ošetření pro případ že infobulbina není určena k vykreslení
         if (InfoCanvas == null)
            return;

         if (InfoVykresleno == 1)
         {
            // Smazání interní proměnné uchovávající informační bublinu
            InfoCanvas.Children.Clear();

            // Nastavení příznakové proměnné pro možnost dalšího vykreslování
            InfoVykresleno = 0;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na daný blok v seznamu.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná udáost</param>
      private void GrafZaznam_MouseMove(object sender, MouseEventArgs e)
      {
         // Ošetření pro případ že infobulbina není určena k vykreslení
         if (InfoCanvas == null)
            return;

         if (InfoVykresleno == 0)
         {
            // Převedení zvoleného objektu zpět na StackPanel
            StackPanel blok = sender as StackPanel;

            // Odstranění prefixu "obr" z názvu bloku
            string IndexZaznamu = blok.Name.Substring(3);

            // Identifikace záznamu na základě indexu objektu (Zjištění o jaký záznam se jedná) a vytvoření informační buliny daného záznamu
            VykresliInfoBublinu(ZaznamyNaJedneStrance[int.Parse(IndexZaznamu)]);

            // Nastavení příznakové proměnné pro zamezení opětovného vykreslování
            InfoVykresleno = 1;
         }
      }

      /// <summary>
      /// Obsluha události při kliknutí na konkrétní blok reprezentující 1 záznam na stránce.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void GrafZaznam_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Převedení zvoleného objektu zpět na StackPanel
         StackPanel blok = sender as StackPanel;

         // Barevné vyznačení vybraného objektu
         blok.Background = Brushes.OrangeRed;

         // Zrušení barevného vyznačení předchozího vybraného objektu
         OznacenyBlok.Background = Brushes.LightBlue;

         // Uložení nově označeného objektu do pomocné proměnné pro možnost následného zrušení jeho označení při označení jiného objektu
         OznacenyBlok = blok;

         // Odstranění prefixu "obr" z názvu bloku
         string IndexPolozky = blok.Name.Substring(3);

         // Identifikace záznamu na základě indexu objektu -> Zjištění o jaký záznam se jedná (přiřazení do VybranyZaznam)
         VybranyZaznam = ZaznamyNaJedneStrance[int.Parse(IndexPolozky)];

         // Předání vybraného záznamu do požadovaného okna
         PredejZaznamOknu(e);
      }

   }
}
