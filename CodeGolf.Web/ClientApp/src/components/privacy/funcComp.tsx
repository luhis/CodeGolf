import { FunctionalComponent, h } from "preact";

interface Props {
    readonly showModal: boolean;
    readonly toggleModal: () => void;
}

const LinkComp: FunctionalComponent<{ readonly toggleModal: (() => void); }> =
    ({ toggleModal }) => <a onClick={toggleModal}>Privacy Policy</a>;

const FuncComp: FunctionalComponent<Readonly<Props>> = ({ showModal, toggleModal }) => showModal ? (
    <div class="modal is-active">
        <div class="modal-background" />
        <div class="modal-content">
            <header class="modal-card-head">
                <p class="modal-card-title">Privacy Policy</p>
                <button class="delete" aria-label="close" onClick={toggleModal} />
            </header>
            <section class="modal-card-body">
                Privacy policy goes here.
            </section>
        </div>
    </div>) : <LinkComp toggleModal={toggleModal} />;

export default FuncComp;
