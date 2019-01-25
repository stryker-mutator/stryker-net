namespace ExampleProject
{
    public class DummyCalc
    {
        public int SomeCalc(int first, int second)
        {
			while(second > 0) {
				first = first + second + 1;
				second--;
			}
			while(1 < 0) {
				// endless looping
			}
            return first;
        }
    }
}
