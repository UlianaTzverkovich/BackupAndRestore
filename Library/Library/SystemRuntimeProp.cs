namespace Library;

public class SystemRuntimeProp
{
    public static int StandartPieceSize { get; } = 64000;
    public static int MaxMemoryUse{ get; } = 500; //макс. объем памяти в мегабайтах
    public static int TimeToWait { get; } = 500; // время ожидания потоками ресурсов в мс
    public static int MaxProcessingThreads { get;} = 6; // кол-во потоков на обработку данных

}