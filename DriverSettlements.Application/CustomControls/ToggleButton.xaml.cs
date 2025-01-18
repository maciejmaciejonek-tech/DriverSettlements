using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DriverSettlements.App.CustomControls;

public partial class ToggleButton : Frame
{
	public ToggleButton()
	{
		InitializeComponent();
	}

    public event EventHandler<EventArgs> Clicked;

    public Color ToggleOffColor { get; set; } = Colors.LightGray;

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


    public static readonly BindableProperty ToggledProperty = BindableProperty.Create(
        propertyName: nameof(Toggled),
        returnType: typeof(bool),
        declaringType: typeof(ButtonControl),
        defaultValue: true,
        propertyChanged: ToggledPropertyChanged
        );

    private static void ToggledPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var controls = (ToggleButton)bindable;
        if (newValue != null)
        {
            bool toggled = (bool)newValue;
            if (toggled)
            {
                controls.SwitchOn();
            }
            else
            {
                controls.SwitchOff();
            }
        }
    }

    private Color? _backgroundColor;

    public void SwitchOn()
    {
        BackgroundColor = _backgroundColor;
    }

    public void SwitchOff()
    {
        _backgroundColor = this.BackgroundColor;
        BackgroundColor = ToggleOffColor;
    }

    public bool Toggled
    {
        get => (bool)GetValue(ToggledProperty);
        set { SetValue(ToggledProperty, value); }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Toggled = !Toggled;
        Clicked?.Invoke(sender, e);
    }
}