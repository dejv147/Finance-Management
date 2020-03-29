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
   ///Třída obsahující logickou vrstvu pro vytvoření a ovládání okenního formuláře NastaveniWindow.xaml
   ///Třída slouží pro nastavení barvy pozadí v oknech aplikace a nastavení zobrazení poznámkového bloku.
   /// </summary>
   public partial class NastaveniWindow : Window
   {
      /// <summary>
      /// Instance hlavního okna pro možnost využití funkcí hlavního okna při 
      /// </summary>
      private MainWindow HlavniOkno;

      /// <summary>
      /// Instance třídy pro správu aplikace
      /// </summary>
      private Spravce_Aplikace SpravaAplikace;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy pozadí v aplikaci
      /// </summary>
      private Brush BarvaPozadi;



      /// <summary>
      /// Konstruktor okenního formuláře pro nastavení aplikace
      /// </summary>
      /// <param name="SpravaAplikace">Instance správce aplkiace</param>
      /// <param name="HlavniOkno">Instance hlavního okna</param>
      public NastaveniWindow(Spravce_Aplikace SpravaAplikace, MainWindow HlavniOkno)
      {
         // Inicializace okenního formuláře
         InitializeComponent();

         // Převzetí instance správce do interní proměnné
         this.SpravaAplikace = SpravaAplikace;

         // Předání instance hlavního okna
         this.HlavniOkno = HlavniOkno;

         // Nastavení úvodního zobrazení zaškrtávcího pole pro možnost zobrazení poznámkového bloku
         UvodniNastaveniPoznamky();

         // Defaultní nastavení barvy pozadí
         BarvaPozadi = HlavniOkno.BarvaPozadi;

         // Určení nastavené barvy pozadí
         UrciNastavenouBarvuPozadi();
      }


      /// <summary>
      /// Identifikace nastavené barvy pozadí
      /// </summary>
      private void UrciNastavenouBarvuPozadi()
      {
         PrvniBarvaRadioButton.IsChecked        =     BarvaPozadi == Brushes.Aqua            ?     true : false;
         DruhaBarvaRadioButton.IsChecked        =     BarvaPozadi == Brushes.Bisque          ?     true : false;
         TretiBarvaRadioButton.IsChecked        =     BarvaPozadi == Brushes.DarkTurquoise   ?     true : false;
         CtvrtaBarvaRadioButton.IsChecked       =     BarvaPozadi == Brushes.HotPink         ?     true : false;
         PataBarvaRadioButton.IsChecked         =     BarvaPozadi == Brushes.Gold            ?     true : false;
         SestaBarvaRadioButton.IsChecked        =     BarvaPozadi == Brushes.Ivory           ?     true : false;
         SedmaBarvaRadioButton.IsChecked        =     BarvaPozadi == Brushes.MediumOrchid    ?     true : false;
         OsmaBarvaRadioButton.IsChecked         =     BarvaPozadi == Brushes.Orange          ?     true : false;
         DevataBarvaRadioButton.IsChecked       =     BarvaPozadi == Brushes.Salmon          ?     true : false;
         DesataBarvaRadioButton.IsChecked       =     BarvaPozadi == Brushes.White           ?     true : false;
         JedenactaBarvaRadioButton.IsChecked    =     BarvaPozadi == Brushes.CadetBlue       ?     true : false;
         DvanactaBarvaRadioButton.IsChecked     =     BarvaPozadi == Brushes.LightBlue       ?     true : false;
      }

      /// <summary>
      /// Úvodní nastavení zaškrtávacího pole
      /// </summary>
      private void UvodniNastaveniPoznamky()
      {
         // Nastavení zaškrtávacího pole dle uloženého nastavení uživatele
         ZobrazitPoznamkuCheckBox.IsChecked = SpravaAplikace.VratZobrazeniPoznamky() == 1 ? true : false;
      }

      /// <summary>
      /// Změna nastavení poznámkového bloku při změně zaškrtávacího pole
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událot</param>
      private void ZobrazitPoznamkuCheckBox_Checked(object sender, RoutedEventArgs e)
      {

         // Nastavení zobrazení poznámkového bloku v hlavním okně
         HlavniOkno.NastavPoznamkovyBlok(1);
      }

      /// <summary>
      /// Změna nastavení poznámkového bloku při změně zaškrtávacího pole
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událot</param>
      private void ZobrazitPoznamkuCheckBox_Unchecked(object sender, RoutedEventArgs e)
      {
         // Zrušení zobrazení poznámkového bloku v hlavním okně
         HlavniOkno.NastavPoznamkovyBlok(0);
      }

      /// <summary>
      /// Událost vyvolána při zavírání okna pro uložení provedených změn.
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         HlavniOkno.NastavBarvuPozadi(BarvaPozadi);
      }


      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrvniBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.Aqua;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DruhaBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.Bisque;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void TretiBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.DarkTurquoise;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void CtvrtaBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.HotPink;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PataBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.Gold;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void SestaBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.Ivory;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void SedmaBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.MediumOrchid;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void OsmaBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.Orange;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DevataBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.Salmon;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DesataBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.White;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void JedenactaBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.CadetBlue;
      }

      /// <summary>
      /// Nastavení zvolené barvy jako barva pozadí
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DvanactaBarvaRadioButton_Checked(object sender, RoutedEventArgs e)
      {
         BarvaPozadi = Brushes.LightBlue;
      }

   }
}
