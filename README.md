# openam_iccard_authmodule
IC Card Authentication module for OpenAM

Contains Authentication Module for OpenAM and Clients to send read information from the IC card.

1. Set up the build module to OpenAM
   For more information, refer to the following URL
   http://docs.forgerock.org/en/openam/11.0.0/dev-guide/#chap-auth-spi
2. Build and Deploy Client Application for your PC
   
   Wondows(64bit): 1. Build Client (Visaul Basic) with Microsoft Visual Studio
                   2. Deploy Client (ex. C:\Program Files (x86)\ICCardAuthn\ICCardAuthn.exe)
                   3. Deploy DLL (ex. C:\Program Files (x86)\ICCardAuthn\Interop.SHDocVw.dll)
                   4. Custom URI Schema set to the windows registry
                       URI: openam://                  
                       Command: C:\Program Files (x86)\ICCardAuthn\ICCardAuthn.exe 
                      For more information, refer to the following URL
                      https://msdn.microsoft.com/ja-jp/library/ie/aa767914(v=vs.85).aspx 
    
   Android: Coming Soon ...

3. Start Authentication with your browzer

   Wondows(64bit): internet explorer