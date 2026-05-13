namespace lw_2;
using CocomoModel;

public partial class MainPage : ContentPage
{
	private readonly Dictionary<string, CocomoModelType> _projectTypeMapping = 
		new Dictionary<string, CocomoModelType>
		{
			{ "Органічний", CocomoModelType.Organic },
			{ "Напівнезалежний", CocomoModelType.SemiDetached },
			{ "Вбудований", CocomoModelType.Embedded }
		};
	
	public MainPage()
	{
		InitializeComponent();
		
		// 1. Инициализация выпадающего списка типов проектов
		CmbProjectType.ItemsSource = _projectTypeMapping.Keys.ToList();
		CmbProjectType.SelectedIndex = 0; // По умолчанию выбираем Organic
		
		// Убираем базовый стиль инпута
		#if IOS || MACCATALYST
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoFocusBorderiOS", (handler, view) =>
		{
		    var textField = handler.PlatformView;

		    // Цвет курсора
		    textField.TintColor = UIKit.UIColor.FromRGB(49, 151, 149);

		    // отключаем системные эффекты фокуса
		    if (OperatingSystem.IsIOSVersionAtLeast(15))
		    {
		        textField.FocusEffect = null;
		    }
		});
		#endif
	}

	private async void OnCalculateClicked(object sender, EventArgs e)
        {
            // Валидация: проверка на пустое поле
            if (string.IsNullOrWhiteSpace(TxtCodeSize.Text))
            {
                await DisplayAlert("Увага", "Будь ласка, введіть обсяг коду (KLOC)!", "OK");
                return;
            }

            // Заменяем точку на запятую (для поддержки разных региональных стандартов девайса)
            string safeInput = TxtCodeSize.Text.Replace('.', ',');

            // Валидация: проверка на число и корректность значения
            if (!double.TryParse(safeInput, out double codeSize) || codeSize <= 0)
            {
                await DisplayAlert("Помилка", "Обсяг коду повинен бути додатним числом!", "OK");
                return;
            }

            // Проверка: выбран ли тип проекта
            if (CmbProjectType.SelectedIndex == -1)
            {
                await DisplayAlert("Увага", "Оберіть тип проекту зі списку!", "OK");
                return;
            }

            // Получаем выбранный тип проекта из Picker (индекс совпадает со значениями Enum)
            CocomoModelType selectedType = (CocomoModelType)CmbProjectType.SelectedIndex;

            try
            {
                // Вызываем расчеты из твоего класса CocomoModelBasic
                double efforts = CocomoModelBasic.GetEfforts(codeSize, selectedType);
                double timeToDevelop = CocomoModelBasic.GetTimeToDevelop(codeSize, selectedType);
                double developers = CocomoModelBasic.GetPersonsToDevelop(codeSize, selectedType);
                double productivity = CocomoModelBasic.GetProductivity(codeSize, selectedType);

                // Выводим результаты на форму с округлением
                LblEfforts.Text = $"{efforts:F2} люд.*міс.";
                LblTimeTo.Text = $"{timeToDevelop:F2} міс.";
                LblDevelopers.Text = $"{developers:F1} чол.";
                LblProductivity.Text = $"{productivity:F3} KLOC/люд.*міс.";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка розрахунку", ex.Message, "OK");
            }
        }
}
