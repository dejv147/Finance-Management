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
   /// Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře PridatPoznamkuWindow.xaml
   /// Okenní formulář slouží k vytvoření textové poznámky a předání poznámky zpět do okna záznamu.
   /// </summary>
   public partial class PridatPoznamkuWindow : Window
   {
      /// <summary>
      /// Text zprávy
      /// </summary>
      private string Poznamka;

      /// <summary>
      /// Instance okna pro přidání záznamu. 
      /// Slouží k navrácení poznámky do okna záznamu.
      /// </summary>
      private PridatZaznamWindow oknoZaznamu;

      /// <summary>
      /// Instance upravovacího okna pro možnost upravit poznámku
      /// </summary>
      private UpravitZaznamWindow OknoUpravitZaznam;

      /// <summary>
      /// Příznakový bit informující zda se okno zavírá křížkem (zobrazení varování), nebo je zavření řízeno voláním funkce pro úmyslné zavření (s uložením dat).
      /// </summary>
      private byte ZavrenoBezUlozeni;

      /// <summary>
      /// Příznakový bit informující zda se jedná o úpravu nebo vytvoření nové poznámky
      /// </summary>
      private byte NovaPoznamka;




      /// <summary>
      /// Konstruktor třídy při otevření z přidávacího okna
      /// </summary>
      public PridatPoznamkuWindow(PridatZaznamWindow OknoZaznamu)
      {
         InitializeComponent();           // Inicializace okenního formuláře
         Poznamka = "";                   // Inicializace proměnné
         ZavrenoBezUlozeni = 1;           // Nastavení hodnoty příznakového bytu pro defaultní nastavení
         this.oknoZaznamu = OknoZaznamu;  // Předání instance okna záznamu
         NovaPoznamka = 1;                // Nastavení příznakového bitu
      }


      /// <summary>
      /// Konstruktor třídy pro možnot upravit poznámku z upravovacího okna
      /// </summary>
      /// <param name="OknoZaznamu">Okno pro úpravu záznamu</param>
      /// <param name="Poznamka">Text poznámky</param>
      public PridatPoznamkuWindow(UpravitZaznamWindow OknoZaznamu, string Poznamka)
      {
         InitializeComponent();
         this.OknoUpravitZaznam = OknoZaznamu;
         PoznamkaTextBox.Text = Poznamka;
         NovaPoznamka = 0;
      }




      /// <summary>
      /// Obsluh tlačítka uloží text zprávy do okna záznamu a zavře okno poznámky.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void UlozitButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Kontrola zda byl zadán nějaký text
            if (!(Poznamka.Length > 0))
               throw new ArgumentException("Zadejte text!");

            // Předání zprávy oknu PridatZaznamWindow
            if (NovaPoznamka == 1) 
               oknoZaznamu.NastavPoznamku(Poznamka);

            // Pokud se jedná o úpravu existující poznámky, předá se do upravovacího okna
            if (NovaPoznamka == 0)
               OknoUpravitZaznam.NastavPoznamku(Poznamka);

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
      /// Načtení textu do interní proměnné
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PoznamkaTextBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         Poznamka = PoznamkaTextBox.Text.ToString();
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
