void setup() 
{
  Serial.begin(9600);
  pinMode(13, OUTPUT);
  pinMode(12, OUTPUT);
  pinMode(11, OUTPUT);
  pinMode(10, OUTPUT);
  pinMode(9, OUTPUT);
  pinMode(8, OUTPUT);
  pinMode(7, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(5, OUTPUT);
  pinMode(4, OUTPUT);
  pinMode(3, OUTPUT);
}

void loop() 
{
  if(Serial.available())
  {
    char currentScene = Serial.read();
    if (currentScene == '1')
    {
      digitalWrite(12, HIGH);
      digitalWrite(11, LOW);
      digitalWrite(10, LOW);
      digitalWrite(9, LOW);
      digitalWrite(8, LOW);
    }
    else if (currentScene == '2')
    {
      digitalWrite(12, LOW);
      digitalWrite(11, HIGH);
      digitalWrite(10, LOW);
      digitalWrite(9, LOW);
      digitalWrite(8, LOW);
    }
    else if (currentScene == '3')
    {
      digitalWrite(12, LOW);
      digitalWrite(11, LOW);
      digitalWrite(10, HIGH);
      digitalWrite(9, LOW);
      digitalWrite(8, LOW);
    }
    else if (currentScene == '4')
    {
      digitalWrite(12, LOW);
      digitalWrite(11, LOW);
      digitalWrite(10, LOW);
      digitalWrite(9, HIGH);
      digitalWrite(8, LOW);
    }
    else if (currentScene == '5')
    {
      digitalWrite(12, LOW);
      digitalWrite(11, LOW);
      digitalWrite(10, LOW);
      digitalWrite(9, LOW);
      digitalWrite(8, HIGH);
    }


    if (currentScene == '6')
    {
      digitalWrite(7, HIGH);
      digitalWrite(6, LOW);
      digitalWrite(5, LOW);
      digitalWrite(4, LOW);
      digitalWrite(3, LOW);
    }
    else if (currentScene == '7')
    {
      digitalWrite(7, LOW);
      digitalWrite(6, HIGH);
      digitalWrite(5, LOW);
      digitalWrite(4, LOW);
      digitalWrite(3, LOW);
    }
    else if (currentScene == '8')
    {
      digitalWrite(7, LOW);
      digitalWrite(6, LOW);
      digitalWrite(5, HIGH);
      digitalWrite(4, LOW);
      digitalWrite(3, LOW);
    }
    else if (currentScene == '9')
    {
      digitalWrite(7, LOW);
      digitalWrite(6, LOW);
      digitalWrite(5, LOW);
      digitalWrite(4, HIGH);
      digitalWrite(3, LOW);
    }
    else if (currentScene == 'n')
    {
      digitalWrite(7, LOW);
      digitalWrite(6, LOW);
      digitalWrite(5, LOW);
      digitalWrite(4, LOW);
      digitalWrite(3, HIGH);
    }
    
  }
}
