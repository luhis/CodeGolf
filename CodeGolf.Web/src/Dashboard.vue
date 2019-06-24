<template>
  <table class="table is-striped is-fullwidth">
    <thead>
      <tr>
        <th></th>
        <th></th>
        <th>Name</th>
        <th>Score</th>
        <th>Time</th>
        <th>Code</th>
      </tr>
    </thead>
    <tbody id="app">
      <tr v-for="result in appData" :key="result.id">
        <td>{{result.rank}}</td>
        <td><figure class="image is-48x48"><img :src="result.avatar" /></figure></td>
        <td>{{result.loginName}}</td>
        <td>{{result.score}}</td>
        <td>{{result.timeStamp}}</td>
        <td>
          <a target="_blank" :href="'/attempt?id=' + result.id" class="button"><i class="fas fa-code"></i>View Code</a>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script>
export default {
  props: ["appData"]
};
</script>