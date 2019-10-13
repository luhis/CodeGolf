
declare module "react-google-recaptcha" {
    import { FunctionalComponent } from "preact";
    const ReCAPTCHA: FunctionalComponent<{ readonly sitekey: string, readonly size: string, readonly onChange: ((response: string) => any) }>;
    export = ReCAPTCHA;
}
