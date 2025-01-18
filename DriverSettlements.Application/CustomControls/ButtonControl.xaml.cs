using System.ComponentModel;
using System.Windows.Input;

namespace DriverSettlements.App.CustomControls;

public partial class ButtonControl : Frame
{
	public ButtonControl()
	{
		InitializeComponent();
	}
    public event EventHandler<EventArgs> Clicked;

    public double FontSize
    {
        get => lblButtonText==null?10:lblButtonText.FontSize;
        set => lblButtonText.FontSize = value;
    }

    public Color TextColor
    {
        get => lblButtonText == null ? Colors.White : lblButtonText.TextColor; 
        set => lblButtonText.TextColor = value;
    }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        propertyName: nameof(CommandParameter),
        returnType: typeof(object),
        declaringType: typeof(ButtonControl),
        defaultBindingMode: BindingMode.TwoWay
        );

    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set { SetValue(CommandParameterProperty, value); }
    }

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        propertyName: nameof(Command),
        returnType: typeof(ICommand),
        declaringType: typeof(ButtonControl),
        defaultBindingMode: BindingMode.TwoWay
        );

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set { SetValue(CommandProperty, value); }
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
		propertyName: nameof(Text),
		returnType: typeof(string),
		declaringType: typeof(ButtonControl),
		defaultValue:"",
		defaultBindingMode: BindingMode.TwoWay
		);

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set { SetValue(TextProperty, value); }
	}

    public static readonly BindableProperty LoadingTextProperty = BindableProperty.Create(
        propertyName: nameof(LoadingText),
        returnType: typeof(string),
        declaringType: typeof(ButtonControl),
        defaultValue: "",
        defaultBindingMode: BindingMode.OneWay
        );

    public string LoadingText
    {
        get => (string)GetValue(LoadingTextProperty);
        set { SetValue(LoadingTextProperty, value); }
    }

    public static readonly BindableProperty IsInProgressProperty = BindableProperty.Create(
        propertyName: nameof(IsInProgress),
        returnType: typeof(bool),
        declaringType: typeof(ButtonControl),
        defaultValue: false,
        propertyChanged: IsInProgressPropertyChanged
        );

    private static void IsInProgressPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var controls = (ButtonControl)bindable;
        if (newValue != null)
        {
            bool isInProgress = (bool)newValue;
            if (isInProgress)
            {
                if (string.IsNullOrWhiteSpace(controls.LoadingText))
                {
                    controls.aiProgress.HorizontalOptions = LayoutOptions.Center;
                    controls.lblButtonText.IsVisible = false;
                }
                else
                    controls.lblButtonText.Text = controls.LoadingText;
            }
            else
            {
                controls.lblButtonText.Text = controls.Text;
                controls.lblButtonText.IsVisible = true;
                controls.aiProgress.HorizontalOptions = LayoutOptions.Start;
            }
        }
    }

    public bool IsInProgress
    {
        get => (bool)GetValue(IsInProgressProperty);
        set { SetValue(IsInProgressProperty, value); }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Clicked?.Invoke(sender, e);
    }
}