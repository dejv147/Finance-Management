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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře PridatPolozkyWindow.xaml
   /// Okenní formulář slouží k vytvoření nové položky a předání vytvořené položky do okna záznamu.
   /// </summary>
   public partial class PridatPolozkyWindow : Window
   {
      /// <summary>
      /// Instance okna pro přidání záznamu. 
      /// Slouží k navrácení seznamu položek do okna záznamu.
      /// </summary>
      private PridatZaznamWindow oknoZaznamu;

      /// <summary>
      /// Instance okna pro úpravu záznamu. 
      /// Slouží k navrácení seznamu položek do okna záznamu.
      /// </summary>
      private UpravitZaznamWindow oknoUpravitZaznam;

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
      /// Vytvořená položka
      /// </summary>
      private Polozka polozka;

      /// <summary>
      /// Položka vybrána ze seznamu
      /// </summary>
      private Polozka VybranaPolozka;

      /// <summary>
      /// Seznam přidaných položek
      /// </summary>
      private ObservableCollection<Polozka> SeznamPolozek;

      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;

      /// <summary>
      /// Instance třídy pro validaci vstupních dat
      /// </summary>
      private Validace validator;

      /// <summary>
      /// Instance třídy pro možnost využití grafických prvků
      /// </summary>
      private Vykreslovani vykreslovani;

      /// <summary>
      /// Instance třídy pro převedení vytvořených položek do grafické podoby
      /// </summary>
      private GrafickePolozky grafickePolozky;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;

      /// <summary>
      /// Příznakový bit informující zda byla provedena úprava položky v případě otevření okna pro úpravu.
      /// </summary>
      public byte PolozkaUpravena;


      /// <summary>
      /// Konstruktor třídy pro inicializaci okna a interních proměných.
      /// </summary>
      /// <param name="OknoZaznamu">Instance okna pro přidání záznamu</param>
      /// <param name="Spravce">Instance správce aplikace</param>
      public PridatPolozkyWindow(PridatZaznamWindow OknoZaznamu, Spravce_Aplikace Spravce)
      {
         InitializeComponent();                       // Inicializace okenního formuláře
         oknoZaznamu = OknoZaznamu;                   // Předání instance okna záznamu
         SpravaAplikace = Spravce;                    // Inicializace správce aplikace
         validator = new Validace();                  // Vytvoření instance třídy pro validaci vstupů od uživatele
         vykreslovani = new Vykreslovani();           // Vytvoření instance třídy pro možnost vykreslování
         ZavrenoBezUlozeni = 1;                       // Nastavení hodnoty příznakového bytu pro defaultní nastavení
         PolozkaUpravena = 0;                         // Nastavení hodnoty příznakového bitu pro defaultní nastavení
         KategoriePolozky = Kategorie.Nevybrano;      // Nastavení defaultní kategorie pro kontrolu výběru

         // Inicializace proměnných pro uložení dat
         Nazev = "";
         Cena = 0;
         Popis = "";
         SeznamPolozek = new ObservableCollection<Polozka>();
         grafickePolozky = new GrafickePolozky(SeznamPolozek, this);
      }

      /// <summary>
      /// Konstruktor pro možnost upravit položky.
      /// </summary>
      /// <param name="OknoZaznamu">Okno pro úpravu záznamu</param>
      /// <param name="SeznamPolozek">Kolekce položek určených k úpravě</param>
      public PridatPolozkyWindow(UpravitZaznamWindow OknoZaznamu, ObservableCollection<Polozka> SeznamPolozek)
      {
         InitializeComponent();
         validator = new Validace();                  // Vytvoření instance třídy pro validaci vstupů od uživatele
         vykreslovani = new Vykreslovani();           // Vytvoření instance třídy pro možnost vykreslování
         ZavrenoBezUlozeni = 1;                       // Nastavení hodnoty příznakového bytu pro defaultní nastavení
         PolozkaUpravena = 1;                         // Nastavení hodnoty příznakového bitu pro defaultní nastavení
         oknoUpravitZaznam = OknoZaznamu;             // Předání instance okna záznamu

         // Inicializace proměnných pro uložení dat
         Nazev = "";
         Cena = 0;
         Popis = "";
         this.SeznamPolozek = SeznamPolozek;
         grafickePolozky = new GrafickePolozky(SeznamPolozek, this);

         // Úvodní vykreslení seznamu položek
            VykresliAktualniSeznamPolozek();
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

            // Zobrazení do boxu načtenou hodnotu (smazání neplatného obsahu)
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
      /// Uložení vytvořeného seznamu položek do kolekce záznamů
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UlozitButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Předání kolekce nově vytvořených položek do okenního formuláře pro vytvoření záznamu
            if (PolozkaUpravena == 0) 
               oknoZaznamu.PridatPolozky(SeznamPolozek);

            // Pokud jsou položky pouze upravovány, předají se upravovacímu oknu záznamu
            if (PolozkaUpravena == 1)
               oknoUpravitZaznam.UlozUpravenePolozky(SeznamPolozek);

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
      /// Přidání nové položky do kolekce položek a aktualizace výpisu
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PridatButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            KontrolaZadanychDat();                                         // Kotrola zadaných dat od uživatele
            polozka = new Polozka(Nazev, Cena, KategoriePolozky, Popis);   // Vytvoření nové položky
            SeznamPolozek.Add(polozka);                                    // Přidání vytvořené položky do seznamu
            VykresliAktualniSeznamPolozek();                               // Aktualizace vykresleného seznamu
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Odebrání vybrané položky z kolekce a aktualizace výpisu
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void OdebratButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Kontrola zda byla vybrána položka pro odstranění
            if (VybranaPolozka == null)
               throw new ArgumentException("Vyberte položku!");

            // Zobrazení varovného okna s načtením zvolené volby
            MessageBoxResult VybranaVolba = MessageBox.Show("Opravdu chcete vybranou položku odstranit?", "Upozornění", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Smazání vybrané položky v případě stisku tlačíka YES
            switch(VybranaVolba)
            {
               case MessageBoxResult.Yes:
                  SeznamPolozek.Remove(VybranaPolozka);  // Smazání vybrané položky ze seznamu
                  VybranaPolozka = null;                 // Smazání reference na vymazanou položku
                  VykresliAktualniSeznamPolozek();       // Aktualizace vykresleného seznamu
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
      /// Metoda pro vykreslení seznamu položek do přidávacího okna
      /// </summary>
      public void VykresliAktualniSeznamPolozek()
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
      /// Zobrazení informační bubliny vybrané položky v přidávacím okně
      /// </summary>
      public void ZobrazInfoPolozky()
      {
         vykreslovani.VykresliPrvek(InfoPolozkaCanvas, grafickePolozky.InfoPolozka);
      }

      /// <summary>
      /// Smazání informační bubliny v přidávacím okně
      /// </summary>
      public void SmazInfoPolozky()
      {
         InfoPolozkaCanvas.Children.Clear();
      }

      /// <summary>
      /// Nalezení vybrané položky v interní kolekci položek na základě porovnání s předanou položkou
      /// </summary>
      /// <param name="polozka">Vybraná položka</param>
      public void VyberPolozku(Polozka polozka)
      {
         foreach (Polozka p in SeznamPolozek)
         {
            if (p.Equals(polozka))
               VybranaPolozka = p;
         }
      }

      /// <summary>
      /// Otevření okna pro úpravu vybrané položky
      /// </summary>
      public void UpravitPolozku()
      {
         UpravitPolozkuWindow upravitPolozkuWindow = new UpravitPolozkuWindow(this, VybranaPolozka);
         upravitPolozkuWindow.ShowDialog();

         // Pokud byly provedeny změny dat, je třeba obnovit vykreslený seznam pro aktualizaci upravené položky
         if (PolozkaUpravena == 1)
         {
            VykresliAktualniSeznamPolozek();
            PolozkaUpravena = 0;
         }
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
