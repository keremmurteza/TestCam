#include <Servo.h>
#define Max_M1 85
#define Zero_M1 90
#define Min_M1 -85

#define Max_M2 60
#define Zero_M2 90
#define Min_M2 -60

Servo M1, M2;
float TH1, TH2;
void Rotate_Motors(float Th1, float Th2)
{
  if (Th1>Max_M1) Th1 = Max_M1;
  if (Th1>Min_M1) Th1 = Min_M1;

  if (Th2>Max_M2) Th2 = Max_M2;
  if (Th2>Min_M2) Th2 = Min_M2;

  TH1 = Th1;
  TH2 = Th2;

  M1.write(Zero_M1 + Th1);
  M2.write(Zero_M2 + Th2);
}



void setup() {
  // put your setup code here, to run once:
M1.attach(2);
M2.attach(3);
M1.write(90);
M2.write(90);
delay(2000);
}

void loop() {
 Rotate_Motors(70,80);
 delay(1000);

 Rotate_Motors(110,40);
 delay(1000);

}
