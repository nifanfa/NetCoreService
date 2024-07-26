# NetCoreService
 Example of making windows services on .net core platform

Build the program by Build->PublishSelectedContent  
![image](https://github.com/user-attachments/assets/4102408a-32a7-488a-9979-1ed227052609)  
  
Make sure the target runtime is *win-x64* otherwise the publish will fail  
![image](https://github.com/user-attachments/assets/8059c262-fba3-4a5e-bc2f-4be36948e72a)  
  
Run the following code to create service  
```bat
sc create {ServiceName} binpath= "{exepath}"
```
Note: ServiceName must be the same as the value in Program.cs  
  
![image](https://github.com/user-attachments/assets/e5a40ff5-fcf5-4d40-a99c-9563a85d0613)  

then you can start your service in taskmgr  
  
this is what happens when you start the service  
![image](https://github.com/user-attachments/assets/8b347f88-43b3-4e25-b5c9-1c98c217d735)



