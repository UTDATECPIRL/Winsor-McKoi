
#include <Ultrasonic.h>

int red_light_pin= 11;
int green_light_pin = 10;
int blue_light_pin = 9;

Ultrasonic ultrasonic(6, 5);

int presence;

void setup() {
  pinMode(red_light_pin, OUTPUT);
  pinMode(green_light_pin, OUTPUT);
  pinMode(blue_light_pin, OUTPUT);
  Serial.begin(9600);
  ultrasonic.setTimeout(20000UL);
}

void loop() {
  presence = ultrasonic.read();
  //Serial.print("Distance: ");
  //Serial.println(presence);
  if (presence>10 && presence<150){
      RGB_color(0, 255, 0); // Green
      //Serial.println("Presence Detected");
  }
  else{
      RGB_color(0, 0, 255); // Blue
      //Serial.println("No Presence Detected");
  }

  if(presence>10 && presence<400){
    Serial.println(presence);
  }
  delay(200);
 
}
  
void RGB_color(int red_light_value, int green_light_value, int blue_light_value)
 {
  analogWrite(red_light_pin, red_light_value);
  analogWrite(green_light_pin, green_light_value);
  analogWrite(blue_light_pin, blue_light_value);
}
