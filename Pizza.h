#include <iostream>

class Pizza{
	public:
	Pizza(){
		std::cout << GetCheeseType() << " is the best Cheese";
	}
	~Pizza(){
		
	}
	
	std::string GetCheeseType(){
		if(cheese_type != "PepperJack")
			cheese_type = "PepperJack";
		return cheese_type;
	}
	
	private:
	int num_toppings = 3;
	std::string cheese_type = "Goat";
	std::string crust_type = "Pan"
}