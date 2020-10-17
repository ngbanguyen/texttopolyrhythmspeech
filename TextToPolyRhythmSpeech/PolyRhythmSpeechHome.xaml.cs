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
using System.Speech.Synthesis;
using System.Timers;

namespace TextToPolyrhythmSpeech
{
    /// <summary>
    /// Interaction logic for PolyRhythmSpeechHome.xaml
    /// </summary>
    public partial class PolyRhythmSpeechHome : Page
    {
		private static System.Timers.Timer aTimer;
		private static SpeechSynthesizer speechSynthesizer;
		private static String[] words;
		private static int index = 0;

		public PolyRhythmSpeechHome()
        {
            InitializeComponent();
			aTimer = new System.Timers.Timer(1000);
			aTimer.Elapsed += OnTimedEvent;
			aTimer.AutoReset = true;
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			/*			PromptBuilder promptBuilder = new PromptBuilder();
						promptBuilder.AppendText("Hello world");

						PromptStyle promptStyle = new PromptStyle();
						promptStyle.Volume = PromptVolume.Soft;
						promptStyle.Rate = PromptRate.Slow;
						promptBuilder.StartStyle(promptStyle);
						promptBuilder.AppendText("and hello to the universe too.");
						promptBuilder.EndStyle();

						promptBuilder.AppendText("On this day, ");
						promptBuilder.AppendTextWithHint(DateTime.Now.ToShortDateString(), SayAs.Date);

						promptBuilder.AppendText(", we're gathered here to learn");
						promptBuilder.AppendText("all", PromptEmphasis.Strong);
						promptBuilder.AppendText("about");
						promptBuilder.AppendTextWithHint("WPF", SayAs.SpellOut);*/

			// Just getting the selected rythm here
			ListBoxItem selected = (ListBoxItem)rythm1.SelectedItem;
			String rythm = (String)selected.Content;

			// Rap the text
			String text = InputTextBox.Text;
			words = text.Split(' ');

			speechSynthesizer = new SpeechSynthesizer();
			speechSynthesizer.Rate = 10;

			if (aTimer.Enabled)
			{
				aTimer.Stop();
			}
			aTimer.Start();
		}

		private static void OnTimedEvent(Object source, ElapsedEventArgs e)
		{
			speechSynthesizer.SpeakAsync(words[index++]);
			if (index == words.Length)
			{
				index = 0;
			}
		}

	}
}
