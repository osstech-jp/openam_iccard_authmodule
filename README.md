# openam_iccard_authmodule
IC Card Authentication module for OpenAM

Contains Authentication Module for OpenAM and Clients to send read information from the IC card. <br/>

1. Set up the build module to OpenAM <br/>
   For more information, refer to the following URL <br/>
   http://docs.forgerock.org/en/openam/11.0.0/dev-guide/#chap-auth-spi <br/>
   By default, matching attribute is uid. you can chose favorite attribute in module settings <br/>
   
2. Build and Deploy Client Application for your PC <br/>
   
   Wondows(64bit): <br/>
                   1. Build Client (Visaul Basic) with Microsoft Visual Studio <br/>
                   2. Deploy Client (ex. C:\Program Files (x86)\ICCardAuthn\ICCardAuthn.exe) <br/>
                   3. Deploy DLL (ex. C:\Program Files (x86)\ICCardAuthn\Interop.SHDocVw.dll) <br/>
                   4. Custom URI Schema set to the windows registry <br/>
                       URI: openam:// <br/>
                       Command: C:\Program Files (x86)\ICCardAuthn\ICCardAuthn.exe <br/>
                      For more information, refer to the following URL <br/>
                      https://msdn.microsoft.com/ja-jp/library/ie/aa767914(v=vs.85).aspx <br/>
                   5. Insert the Card Reader to the Windows PC <br/>
                      I tried to use the below card reader <br/>
                      http://www.acs.com.hk/en/products/3/acr122u-usb-nfc-reader/ <br/>
                   By default, send idm of ic card. you can modify program to change send data. <br/>
                   Also, this application open the internet explorer by default. <br/>

   Android: Coming Soon ... <br/>

3. Start Authentication with your browser <br/>
