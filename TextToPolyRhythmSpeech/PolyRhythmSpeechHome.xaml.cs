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
using System.Text.RegularExpressions;

namespace TextToPolyrhythmSpeech
{
    /// <summary>
    /// Interaction logic for PolyRhythmSpeechHome.xaml
    /// </summary>
    public partial class PolyRhythmSpeechHome : Page
    {
		private static SpeechSynthesizer speechSynthesizer;
		private static readonly int speakRate = 1;
		private static readonly int defaultBPM = 120;
		private static readonly int maxBPM = 240;
		private static readonly int minBPM = 60;
		private static readonly int defaultTimeSpan = 2500; // 100 nanoseconds
		private static String[] words;
		private static Hyphenator hyphenator = new Hyphenator(HyphenatePatternsLanguage.EnglishUs, " ");

		public PolyRhythmSpeechHome()
        {
            InitializeComponent();

            speechSynthesizer = new SpeechSynthesizer
            {
                Rate = speakRate
            };
		}

		[Obsolete]
        private void Button1_Click(object sender, RoutedEventArgs e)
		{
			words = hyphenator.HyphenateText(InputTextBox.Text).Split(' ');

			int metronome1Notes, metronome2Notes;

			if (int.TryParse(Meter1Notes.Text, out metronome1Notes) && int.TryParse(Meter2Notes.Text, out metronome2Notes)) {
				int notes = LCM(metronome1Notes, metronome2Notes);
				List<int> beat = new List<int>();

				for (int i = 0; i < notes; ++i)
                {
					if (i % metronome1Notes == 0 || i % metronome2Notes == 0)
                    {
						beat.Add(i);
                    }
                }

				List<TimeSpan> space = new List<TimeSpan>();
				for (int i = 0; i < beat.Count - 1; ++i)
                {
					space.Add(new TimeSpan(defaultTimeSpan * (beat[i + 1] - beat[i])));
                }
				space.Add(new TimeSpan(defaultTimeSpan * (notes - beat[beat.Count - 1])));

				PromptBuilder promptBuilder = new PromptBuilder();

				PromptStyle promptStyle = new PromptStyle();
				promptStyle.Volume = PromptVolume.Soft;
				promptStyle.Rate = PromptRate.Medium;
				promptBuilder.StartStyle(promptStyle);

				// Not the best at splitting but it works
				// P E O P L E
				words = hyphenator.HyphenateText(InputTextBox.Text).Split(' ');

				int timespanIndex = 0;
				foreach (string s in words)
				{
					promptBuilder.AppendText(s);
					promptBuilder.AppendBreak(space[timespanIndex++]);
					if (timespanIndex >= space.Count())
                    {
						timespanIndex = 0;
                    }
				}

				promptBuilder.EndStyle();

				speechSynthesizer.SpeakAsync(promptBuilder);
			}
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
			speechSynthesizer.SpeakAsyncCancelAll();
		}

		// Filtering key input to only key number
		private void BPMTextBox_KeyDown(object sender, KeyEventArgs e)
        {
			TextBox textBox = (TextBox) sender;
			bool isNumeric = (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
			bool isIncr = (e.Key == Key.Add);
			bool isDecr = (e.Key == Key.Subtract);
			if (isNumeric || isIncr || isDecr)
            {
				if (isIncr || isDecr)
                {
					string original = textBox.Text;
					int result;
					if (int.TryParse(original, out result))
                    {
						result = (isIncr) ? (result + 1) : (result - 1);
						result = (result > maxBPM) ? maxBPM : (result < minBPM) ? minBPM : result;
						textBox.Text = result.ToString();
					} else
                    {
						// Paranoid
						textBox.Text = defaultBPM.ToString();
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
	}
}
