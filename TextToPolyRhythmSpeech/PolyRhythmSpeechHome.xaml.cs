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
using System.Numerics;

// Pogger library
using NHyphenator.Loaders;
using NHyphenator;




namespace TextToPolyrhythmSpeech
{
    /// <summary>
    /// Interaction logic for PolyRhythmSpeechHome.xaml
    /// </summary>
    public partial class PolyRhythmSpeechHome : Page
    {
		private static System.Timers.Timer metronome;
		private static SpeechSynthesizer speechSynthesizer;
		private static int index = 0;
		private static readonly int speakRate = 1;
		private static readonly String defaultBPM = "120";
		private static readonly int maxBPM = 200;
		private static readonly int minBPM = 10;

		private static bool pleaseStop = false;
		private static String[] words;
		private static Hyphenator hyphenator = new Hyphenator(HyphenatePatternsLanguage.EnglishUs, " ");


		public PolyRhythmSpeechHome()
        {
            InitializeComponent();

			pleaseStop = false;

			metronome = new System.Timers.Timer();
			metronome.Elapsed += OnTimedEvent;
			metronome.AutoReset = true;

            speechSynthesizer = new SpeechSynthesizer
            {
                Rate = speakRate
            };
		}

		[Obsolete]
        private void Button1_Click(object sender, RoutedEventArgs e)
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
			// ListBoxItem selected = (ListBoxItem)rythm1.SelectedItem;
			// String rythm = (String)selected.Content;

			// Rap the text

			// Hyphenate the text to split syllables in a normal way
			// Instead of taking four fking seconds to say aviation

			if (metronome.Enabled)
            {
				metronome.Stop();
			}

			index = 0;
			pleaseStop = false;

			words = hyphenator.HyphenateText(InputTextBox.Text).Split(' ');

			int metronome1BPM, metronome2BPM, metronome1Notes, metronome2Notes;

			if (int.TryParse(BPMTextBox1.Text, out metronome1BPM) && int.TryParse(BPMTextBox2.Text, out metronome2BPM) &&
				int.TryParse(Meter1Notes.Text, out metronome1Notes) && int.TryParse(Meter2Notes.Text, out metronome2Notes)) {
				int notes = LCM(metronome1Notes, metronome2Notes);
				metronome.Interval = 500 / notes;

				// Wat now
				speechSynthesizer.Rate = 1 + Math.Max(metronome1BPM, metronome2BPM) / 45;

				metronome.Start();
			}
		}

		private static void OnTimedEvent(Object source, ElapsedEventArgs e)
		{
			if (pleaseStop)
            {
				metronome.Stop();
				return;
            }
			speechSynthesizer.SpeakAsync(words[index++]);
			if (index >= words.Length)
            {
				index = 0;
				pleaseStop = true;
            }
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
			pleaseStop = true;
			index = 0;
		}

        // Filtering key input to only key number
        private void BPMTextBox_KeyDown(object sender, KeyEventArgs e)
        {
			if (sender != BPMTextBox1 || sender != BPMTextBox2)
            {
				// Wat
				return;
            }
			TextBox textBox = (TextBox) sender;
			bool isNumeric = (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
			bool isIncr = (e.Key == Key.Add);
			bool isDecr = (e.Key == Key.Subtract);
			if (isNumeric || isIncr || isDecr)
            {
				if (isIncr || isDecr)
                {
					string original = BPMTextBox1.Text;
					int result;
					if (int.TryParse(original, out result))
                    {
						result = (isIncr) ? (result + 1) : (result - 1);
						result = (result > maxBPM) ? maxBPM : (result < minBPM) ? minBPM : result;
						textBox.Text = result.ToString();
					} else
                    {
						// Paranoid
						textBox.Text = defaultBPM;
					}
					// Prevents default handler action after this
					e.Handled = true;
                }
            } else {
				// No actions if not numeric or add/subtract
				e.Handled = true;
			}
		}

		int LCM(int a, int b)
		{
			int larger = (a > b) ? a : b;
			int smaller = (a > b) ? b : a;
			int candidate = larger;
			while (candidate % smaller != 0)
            {
				candidate += larger;
            }
			return candidate;
		}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
			PromptBuilder promptBuilder = new PromptBuilder();
			promptBuilder.AppendText("Hello world");
			TimeSpan break1 = new TimeSpan(50);

			PromptStyle promptStyle = new PromptStyle();
			promptStyle.Volume = PromptVolume.Soft;
			promptStyle.Rate = PromptRate.Slow;
			promptBuilder.StartStyle(promptStyle);
			promptBuilder.AppendText("and");
			promptBuilder.AppendBreak(break1);
			promptBuilder.AppendText("and");
			promptBuilder.AppendBreak(break1);
			promptBuilder.AppendText("and");
			promptBuilder.AppendBreak(break1);
			promptBuilder.AppendText("and");
			promptBuilder.AppendBreak(break1);
			promptBuilder.EndStyle();

			speechSynthesizer.Speak(promptBuilder);
		}
	}
}
