using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorVoice
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create a new SpeechRecognitionEngine instance.
            SpeechRecognizer recognizer = new SpeechRecognizer();

            // Create a simple grammar that recognizes "red", "green", or "blue".
            Choices colors = new Choices();
            colors.Add(new string[] { "mirror" });

            // Create a GrammarBuilder object and append the Choices object.
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(colors);

            // Create the Grammar instance and load it into the speech recognition engine.
            Grammar g = new Grammar(gb);
            recognizer.LoadGrammar(g);

            // Register a handler for the SpeechRecognized event.
            recognizer.SpeechRecognized +=
              new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
        }

        // Create a simple handler for the SpeechRecognized event.
        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
            Grammar dictationGrammar = new DictationGrammar();
            recognizer.LoadGrammar(dictationGrammar);

            try
            {
                recognizer.SetInputToDefaultAudioDevice();
                RecognitionResult result = recognizer.Recognize();
                if (result == null)
                {
                    MessageBox.Show("Could hear what u said!");
                    return;
                }
                parseRecognizedText(result.Text.ToLower());
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show("Could not recognize input from default aduio device. Is a microphone or sound card available?" + exception.Source + exception.Message);
            }
            finally
            {
                recognizer.UnloadAllGrammars();
            }
        }

        void parseRecognizedText(String recognizedText)
        {
            string[] stringArray = { "agenda", "appointment" };
            int selectedIndex;
            for (selectedIndex = 0; selectedIndex < stringArray.Length; selectedIndex++)
            {
                if (recognizedText.Contains(stringArray[selectedIndex]))
                {
                    switch (selectedIndex)
                    {
                        case 0:
                            parseAgendaCommandText(recognizedText);
                            break;
                        case 1:
                            parseAppointmentCommandText(recognizedText);
                            break;
                        default:
                            break;
                    }
                    break;
                }
            }
        }

        void parseAgendaCommandText(String agendaText)
        {
            if (agendaText.Contains("open"))
            {
                MessageBox.Show("Agenda opened");
            }
            else if (agendaText.Contains("close"))
            {
                MessageBox.Show("Agenda closed");
            }
        }

        void parseAppointmentCommandText(String appointmentText)
        {
            string[] monthArray = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
            int selectedIndex;
            bool monthFound = false;
            for (selectedIndex = 0; selectedIndex < monthArray.Length; selectedIndex++)
            {
                if (appointmentText.Contains(monthArray[selectedIndex]))
                {
                    monthFound = true;
                    break;
                }
            }
            if (!monthFound)
            {
                if (appointmentText.Contains("today"))
                {
                    MessageBox.Show("Make appointment today");
                }
            }
            else
            {
                MessageBox.Show("Make appointment in " + monthArray[selectedIndex]);
            }
        }
    }
}
