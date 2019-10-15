import { FunctionalComponent, h } from "preact";

interface Props {
    readonly showModal: boolean;
    readonly toggleModal: () => void;
    readonly signOutFunc: () => Promise<void>;
}

const LinkComp: FunctionalComponent<{ readonly toggleModal: (() => void); }> =
    ({ toggleModal }) => <a class="navbar-item" onClick={toggleModal}>Sign Out</a>;

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ showModal, toggleModal, signOutFunc }) => showModal ? (
    <div class="modal is-active">
        <div class="modal-background" />
        <div class="modal-content">
            <header class="modal-card-head">
                <p class="modal-card-title">Sign Out</p>
                <button class="delete" aria-label="close" onClick={toggleModal} />
            </header>
            <section class="modal-card-body">
                Are you sure you want to sign out?
            </section>
            <footer class="modal-card-foot">
                <button class="button is-success" onClick={signOutFunc}>Sign Out</button>
                <button class="button" onClick={toggleModal}>Cancel</button>
            </footer>
        </div>
    </div>) : <LinkComp toggleModal={toggleModal} />;

export default FuncComp;
