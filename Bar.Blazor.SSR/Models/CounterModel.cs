using System.ComponentModel.DataAnnotations;

    public class CounterModel
    {
    //[Range(1, 10, ErrorMessage = "Значение должно быть целым числом от 1 до 10.")]
    public int CounterValue { get; set; } = 0;
    }
