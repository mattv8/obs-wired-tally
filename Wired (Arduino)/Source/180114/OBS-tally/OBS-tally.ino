void setup() 
{
  Serial.begin(9600);
  pinMode(13, OUTPUT);
  pinMode(12, OUTPUT);
  pinMode(10, OUTPUT);
  pinMode(8, OUTPUT);
  pinMode(6, OUTPUT);
}

void loop() 
{
  if(Serial.available())
  {
    char currentScene = Serial.read();
    if (currentScene == '1')
    {
      digitalWrite(12, HIGH);
      digitalWrite(10, LOW);
      digitalWrite(8, LOW);
      digitalWrite(6, LOW);
    }
    else if (currentScene == '2')
    {
      digitalWrite(10, HIGH);
      digitalWrite(12, LOW);
      digitalWrite(8, LOW);
      digitalWrite(6, LOW);
    }
    else if (currentScene == '3')
    {
      digitalWrite(10, LOW);
      digitalWrite(12, LOW);
      digitalWrite(8, HIGH);
      digitalWrite(6, LOW);
    }
    else if (currentScene == '4')
    {
      digitalWrite(10, LOW);
      digitalWrite(12, LOW);
      digitalWrite(8, LOW);
      digitalWrite(6, HIGH);
    }
    else
    {
      digitalWrite(10, LOW);
      digitalWrite(12, LOW);
      digitalWrite(8, LOW);
      digitalWrite(6, LOW);
    }
    
  }
}
