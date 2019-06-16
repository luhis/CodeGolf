<template>
  <table class="table is-striped is-fullwidth">
    <thead>
      <tr>
        <th>Name</th>
        <th>Score</th>
        <th>Time</th>
        <th>Code</th>
      </tr>
    </thead>
    <tbody id="app">
      <tr v-for="result in appData" :key="result.id">
        <td>{{result.userId}}</td>
        <td>{{result.score}}</td>
        <td>{{result.timeStamp}}</td>
        <td>
          <a target="_blank" :href="'/api/code/attempt?id=' + result.id"><i class="fas fa-code"></i></a>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script>
export default {
  props: ["appData"],
  watch: {
    stuff() {
      console.log("changed");
    }
  }
};
</script>