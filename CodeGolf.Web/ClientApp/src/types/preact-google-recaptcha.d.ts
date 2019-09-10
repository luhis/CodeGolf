
declare module "preact-google-recaptcha" {
    import { FunctionalComponent } from "preact";
    const ReCAPTCHA: FunctionalComponent<{ sitekey: string, size: string, onChange: ((response: string) => any) }>;
    export = ReCAPTCHA;
}
