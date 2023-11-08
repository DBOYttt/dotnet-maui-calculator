namespace Calculator;

public partial class MainPage : ContentPage
{
    
    public MainPage()
    {
        InitializeComponent();
        OnClear(this, null); // dla bezpieczeństwa restartuje wartości początkowe

    }

    string currentEntry = "";
    int currentState = 1;
    string mathOperator;
    double firstNumber, secondNumber;
    string decimalFormat = "N0";

    private async Task AnimateButton(Button button)
    {
        await button.ScaleTo(0.95, 50); // Animacja przycisków await użyam do czekania aż przycisk zostanie zeskalowany(inaczej mówiąc oczekuje na np.button.ScaleTo)
        await button.FadeTo(0.5, 50);  
        await button.ScaleTo(1, 50);   
        await button.FadeTo(1, 50);    
    }


    async void OnSelectNumber(object sender, EventArgs e) // wypisanie klikniętej cyfry na wyświetlaczu 
    {

        Button button = (Button)sender;
        await AnimateButton(button); // animacja przycisku 

        string pressed = button.Text;

        currentEntry += pressed; // zliczanie aktualnych danych wejściowych

        if ((this.resultText.Text == "0" && pressed == "0")
            || (currentEntry.Length <= 1 && pressed != "0") // jeśli na ekranie jest na ekranie wartość dłogości 1 bądź mniej i jest inny od zera to czyścimy wyświetlacz tnz. chodzi o to że jak jest 0 i wpiszemy 1 to jest jeden nie 01
            || currentState < 0)
        {
            this.resultText.Text = "";
            if (currentState < 0)
                currentState *= -1;
        }

        if (pressed == "." && decimalFormat != "N2") // liczby po kropce a dokładniej ich format, nie wiem co znaczy N2 znalazłem to gdzieś na internecie (●'◡'●)
        {
            decimalFormat = "N2";
        }

        this.resultText.Text += pressed;
    }

    async void OnSelectOperator(object sender, EventArgs e) // używanie operatora matematycznego, ps funkcja asynchroniczna na potrzebe użycia await
    {
        LockNumberValue(resultText.Text); // pobieranie aktualnej wartości kalkulatora

        currentState = -2;
        Button button = (Button)sender;
        await AnimateButton(button);
        string pressed = button.Text;
        mathOperator = pressed;  // odwołanie (albo raczej przekazanie) operatora do pliku Calculator.cs          
    }

    private void LockNumberValue(string text) // praktycznie to samo co przekształcanie stringa w liczbe 
    {
        // TO:DO dodać wyniki po przecinku
        double number;
        if (double.TryParse(text, out number))
        {
            if (currentState == 1)
            {
                firstNumber = number;
            }
            else
            {
                secondNumber = number;
            }

            currentEntry = string.Empty;
        }
    }

    void OnClear(object sender, EventArgs e) // czyszczenie
    {
        firstNumber = 0;
        secondNumber = 0;
        currentState = 1;
        decimalFormat = "N0";
        this.resultText.Text = "0";
        currentEntry = string.Empty;
    }


    async void OnCalculate(object sender, EventArgs e)
    {
        if (currentState == 2)
        {
            if (secondNumber == 0)
                LockNumberValue(resultText.Text);

            double result = Calculator.Calculate(firstNumber, secondNumber, mathOperator);
            this.CurrentCalculation.Text = $"{firstNumber} {mathOperator} {secondNumber}";

            // Sprawdzenie, czy sender jest typu Button i wykonanie animacji jeśli tak
            if (sender is Button button)
            {
                await AnimateButton(button);
            }

            // Ustawienie formatu dziesiętnego przed wyświetleniem wyniku
            decimalFormat = mathOperator switch
            {
                "√" => "N2", // Dla pierwiastka używamy dwóch miejsc po przecinku
                "x²" => "N2", // Dla kwadratu również możemy chcieć mieć dwie cyfry po przecinku
                _ => decimalFormat // Dla pozostałych operacji zachowujemy obecny format
            };
            this.resultText.Text = result.ToTrimmedString(decimalFormat);
            firstNumber = result; // Przypisanie wyniku do firstNumber dla dalszych obliczeń
            secondNumber = 0;
            currentState = 1; // Reset stanu do domyślnego
            currentEntry = string.Empty;
        }
    }




    async void OnNegative(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        await AnimateButton(button);
        if (currentState == 1)
        {
            secondNumber = -1;
            mathOperator = "×";
            currentState = 2;
            OnCalculate(this, null);
        }
    }

    async void OnPercentage(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        await AnimateButton(button);
        if (currentState == 1)
        {
            LockNumberValue(resultText.Text);
            decimalFormat = "N2";
            secondNumber = 0.01;
            mathOperator = "×";
            currentState = 2;
            OnCalculate(this, null);
        }
    }

    async void OnRoot(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        await AnimateButton(button);
        if (currentState == 1)
        {
            LockNumberValue(resultText.Text);
            mathOperator = "√";
            currentState = 2;
            OnCalculate(this, null);
        }
    }

    async void OnSquare(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        await AnimateButton(button);
        if (currentState != 2) // Sprawdza, czy obecnie nie jesteśmy w trakcie operacji
        {
            LockNumberValue(resultText.Text);
            firstNumber = firstNumber * firstNumber; // Przypisujemy wynik kwadratu do firstNumber
            this.resultText.Text = firstNumber.ToString(decimalFormat);
            currentEntry = firstNumber.ToString(); // Ustawiamy currentEntry na nową wartość
            currentState = 1; // Ustawiamy currentState na 1, aby można było kontynuować obliczenia
        }
    }



}
