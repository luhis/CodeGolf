import { FunctionalComponent, h } from "preact";

import { Round } from "../../types/types";

interface Props { readonly hide: () => void, readonly save: () => void, readonly challenges: ReadonlyArray<Round> }

const Modal: FunctionalComponent<Props> = ({ hide, save }) =>
  (<div class="modal is-active">
    <div class="modal-background" />
    <div class="modal-content">
      <header class="modal-card-head">
        <p class="modal-card-title">Create New Game</p>
        <button class="delete" aria-label="close" onClick={hide} />
      </header>
      <section class="modal-card-body">
        <div class="field">
          <label class="label">Access Code</label>
          <div class="control">
            <input class="input is-primary" type="text" />
          </div>
        </div>
      </section>
      <footer class="modal-card-foot">
        <button class="button is-success" onClick={save}>Save changes</button>
        <button class="button" onClick={hide}>Cancel</button>
      </footer>
    </div>
  </div>);

export default Modal;
